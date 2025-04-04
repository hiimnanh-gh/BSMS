using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI.Models;


public class OrderRequest
{
    public string UserId { get; set; }  // ID của người dùng
    public string Address { get; set; } // Địa chỉ người dùng

    public DateTime OrderDate { get; set; }  // Ngày đặt hàng
    public decimal TotalAmount { get; set; }  // Tổng giá trị đơn hàng

    public string PaymentMethod { get; set; } // Phương thức thanh toán

    // Danh sách các mục trong đơn hàng (OrderItems)
    public List<OrderItemRequest> OrderItems { get; set; }
}


