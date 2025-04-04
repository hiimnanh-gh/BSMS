using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI.Models;

public class OrderItemRequest
{
    public string BookId { get; set; }  // ID của sách

    public int Quantity { get; set; }  // Số lượng của sách trong đơn hàng
}

