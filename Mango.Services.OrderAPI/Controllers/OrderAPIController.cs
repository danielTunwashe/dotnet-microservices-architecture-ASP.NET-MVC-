using AutoMapper;
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.Dto;
using Mango.Services.OrderAPI.Models.Dto.EmailDto;
using Mango.Services.OrderAPI.Service.IService;
using Mango.Services.OrderAPI.Utility;
using Mango.Web.Models.PayStackModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace Mango.Services.OrderAPI.Controllers
{
    [Route("api/Order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        protected ResponseDto _response;
        private IMapper _mapper;
        private readonly AppDbContext _db;
        private IProductService _productService;
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly IEmailService _emailService;

        public OrderAPIController(AppDbContext db, IProductService productService, IMapper mapper, IConfiguration config, HttpClient httpClient, IEmailService emailService)
        {
            _db = db;
            _mapper = mapper;
            _productService = productService;
            this._response = new ResponseDto();
            _config = config;
            _httpClient = httpClient;
            _emailService = emailService;
        }


        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDto> CreateOrder([FromBody] CartDto cartDto)
        {
            try
            {
                var orderHeaderDto = _mapper.Map<OrderHeaderDto>(cartDto.CartHeader);
                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.Status = SD.Status_Pending;
                orderHeaderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDto>>(cartDto.CartDetails);

                var orderCreated =  _db.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;
                await _db.SaveChangesAsync();

                orderHeaderDto.OrderHeaderId = orderCreated.OrderHeaderId;
                _response.Result = orderHeaderDto;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }

        [Authorize]
        [HttpGet("GetOrders")]
        public async Task<ResponseDto> GetOrders(string? userId = "")
        {
            try
            {
                IEnumerable<OrderHeader> orderHeaders;
                if(User.IsInRole(SD.Role_Admin))
                {
                    orderHeaders = await _db.OrderHeaders
                        .Include(o => o.OrderDetails)
                        .OrderByDescending(o => o.OrderHeaderId)
                        .ToListAsync();
                }
                else
                {
                    orderHeaders = await _db.OrderHeaders
                        .Where(o => o.UserId == userId)
                        .Include(o => o.OrderDetails)
                        .OrderByDescending(o => o.OrderHeaderId)
                        .ToListAsync();
                }

                _response.Result = _mapper.Map<IEnumerable<OrderHeaderDto>>(orderHeaders);
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }

        [Authorize]
        [HttpGet("GetOrderById/{OrderId}")]
        public async Task<ResponseDto> GetOrderById([FromRoute] int OrderId)
        {
            try
            {
                var orderHeader = await _db.OrderHeaders
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.OrderHeaderId == OrderId);

                if (orderHeader == null)
                {
                    throw new Exception("orderHeader not found");
                }

                var orderHeaderDto = _mapper.Map<OrderHeaderDto>(orderHeader);

                _response.Result = orderHeaderDto;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }

        [Authorize]
        [HttpPut("UpdateOrder")]
        public async Task<ResponseDto> UpdateOrder([FromBody] UpdateOrderDto updateOrder)
        {
            try
            {
                var orderHeaderExist = await _db.OrderHeaders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.OrderHeaderId == updateOrder.OrderId);

                if (orderHeaderExist == null)
                {
                    throw new Exception("orderHeader not found");
                }

                orderHeaderExist.Status = SD.Status_Approved;
                orderHeaderExist.PaymentIntentId = updateOrder.Reference;
                orderHeaderExist.StripeSessionId = updateOrder.Reference;

                _db.OrderHeaders.Update(orderHeaderExist);
                await _db.SaveChangesAsync();

                var orderHeaaderMapped = _mapper.Map<OrderHeaderDto>(orderHeaderExist);

                _response.Result = orderHeaaderMapped;
                _response.Message = "OrderHeader Table Updated Successfully...";
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }


        [Authorize]
        [HttpPut("UpdateOrderStatus")]
        public async Task<ResponseDto> UpdateOrderStatus([FromBody] UpdateOrderStatusInput input)
        {
            try
            {
                var orderHeader = await _db.OrderHeaders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.OrderHeaderId == input.OrderId);

                if (orderHeader == null)
                {
                    throw new Exception("orderHeader not found");
                }

                if(input.NewStatus == SD.Status_Cancelled)
                {
                    //Need to implement refund logic here later...
                    // we can also add more conditions in else if for other status

                }
                orderHeader.Status = input.NewStatus;

                _db.OrderHeaders.Update(orderHeader);
                await _db.SaveChangesAsync();
                var orderHeaaderMapped = _mapper.Map<OrderHeaderDto>(orderHeader);


                _response.Result = orderHeaaderMapped;
                _response.Message = "OrderHeader Status Updated Successfully...";
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("initialize")]
        public async Task<ResponseDto> Initialize([FromBody] InitializePaymentRequestDto Request)
        {
            try
            {
                var secretKey = _config["Paystack:SecretKey"];
                var callbackUrl = Request.CallbackUrl ?? _config["Paystack:CallbackUrl"];

                var payload = new
                {
                    email = Request.Email,
                    amount = (int)(Request.Amount * 100), // Paystack expects amount in kobo
                    callback_url = callbackUrl
                };

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", secretKey);
                var response = await _httpClient.PostAsJsonAsync("https://api.paystack.co/transaction/initialize", payload);

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(json);

                var responseDto =  new InitializePaymentResponseDto
                {
                    AuthorizationUrl = result.data.authorization_url,
                    AccessCode = result.data.access_code,
                    Reference = result.data.reference
                    
                };

                _response.Result = responseDto;
                _response.Message = $"Successful";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return _response;
        }

        [Authorize]
        [HttpGet("verify/{reference}")]
        public async Task<ResponseDto> Verify([FromRoute] string reference)
        {
            try
            {
                var secretKey = _config["Paystack:SecretKey"];

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", secretKey);
                var response = await _httpClient.GetAsync($"https://api.paystack.co/transaction/verify/{reference}");

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(json);

                var responseDto =  new VerifyPaymentResponseDto
                {
                    Status = result.data.status == "success",
                    GatewayResponse = result.data.gateway_response,
                    Reference = result.data.reference,
                    Message = result.message
                };

                _response.Result = responseDto;
                _response.Message = responseDto.Message;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return _response;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            try
            {
                // Read raw JSON body
                using var reader = new StreamReader(Request.Body);
                var requestBody = await reader.ReadToEndAsync();

                // Get signature from headers
                var signature = Request.Headers["X-Paystack-Signature"].FirstOrDefault();

                //Pass in my secret key
                var secretKey = _config["Paystack:SecretKey"];

                //// 🔐 Verify signature
                if (!IsValidSignature(requestBody, signature, secretKey))
                {
                    throw new Exception("Sinature Verification Failed..(Post request not from paystack..)");
                }

                var webhook = JsonConvert.DeserializeObject<PaystackWebhookResponseDto>(requestBody);

                if (webhook == null)
                {
                    throw new Exception("Invalid webhook payload");
                }

                string reference = webhook.Data.reference;
                string eventType = webhook.Event;

                //// Verify payment with Paystack API
                var verification = await Verify(reference);
                var verificationResult = JsonConvert.DeserializeObject<VerifyPaymentResponseDto>(JsonConvert.SerializeObject(verification.Result));

                var orderHeader = await _db.OrderHeaders.FirstOrDefaultAsync(o => o.StripeSessionId == reference);

                if ((eventType == "charge.success") && (verificationResult.Status == true))
                {
                    if (orderHeader == null)
                    {
                        throw new Exception("Order not found");
                    }
                    orderHeader.Status = SD.Status_Approved;
                    orderHeader.PaymentIntentId = reference;
                    orderHeader.StripeSessionId = reference;

                    _db.OrderHeaders.Update(orderHeader);
                    await _db.SaveChangesAsync();

                    var emailPayload = new SendConfirmationEmailInput
                    {
                        ToEmail = orderHeader.Email,
                        Order = new OrderEmailRequest
                        {
                            OrderHeaderId = orderHeader.OrderHeaderId,
                            OrderTotal = orderHeader.OrderTotal,
                            Name = orderHeader.Name,
                            OrderDetails = orderHeader.OrderDetails.Select(d => new OrderItemDto
                            {
                                Count = d.Count,
                                Price = d.Price,
                                ProductName = d.ProductName
                            }).ToList()
                        }
                    };

                    await _emailService.SendOrderConfirmationEmailAsync(emailPayload);
                }
                else if ((eventType == "charge.failed") && (verificationResult.Status == false))
                {
                    orderHeader.Status = SD.Status_Cancelled;
                    orderHeader.PaymentIntentId = reference;
                    orderHeader.StripeSessionId = reference;

                    _db.OrderHeaders.Update(orderHeader);
                    await _db.SaveChangesAsync();
                }

                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        private bool IsValidSignature(string requestBody, string signature, string secret)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secret));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(requestBody));
            var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            return hash == signature?.ToLower();
        }
    }
}
