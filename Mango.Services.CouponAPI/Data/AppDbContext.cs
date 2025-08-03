using Mango.Services.CouponAPI.Models;

namespace Mango.Services.CouponAPI.Data;

public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public AppDbContext(Microsoft.EntityFrameworkCore.DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public Microsoft.EntityFrameworkCore.DbSet<Coupon> Coupons { get; set; }

    protected override void OnModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Coupon>().HasData(
            new Coupon
            {
                CouponId = 1,
                CouponCode = "10OFF",
                DiscountAmount = 10.0,
                MinAmount = 50
            },
            new Coupon
            {
                CouponId = 2,
                CouponCode = "20OFF",
                DiscountAmount = 20.0,
                MinAmount = 100
            }
        );
    }
}
