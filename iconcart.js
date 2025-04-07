const API_URL = "http://localhost:5178/CartItem";
document.addEventListener("DOMContentLoaded", () => {
  fetchBooks();
});

window.addEventListener("load", function () {
  // Xóa checkoutItems mỗi khi trang cart.html được mở lại
  sessionStorage.removeItem("checkoutItems");

  // Sau khi xóa, bạn có thể tiếp tục thực hiện các thao tác khác như tải giỏ hàng
  fetchCart(); // Hoặc gọi hàm tải giỏ hàng của bạn
});

// 🛒 Load giỏ hàng
async function fetchCart() {
  try {
    const cartID = sessionStorage.getItem("cartID");

    const response = await fetch(`${API_URL}/GetList?cartID=${cartID}`);

    if (!response.ok) throw new Error("Không thể lấy giỏ hàng");

    const cartItems = await response.json();
    console.log("Giỏ hàng:", cartItems); // Debug kiểm tra
    console.log("Giỏ hàng:", cartItems);

    renderCart(cartItems);
  } catch (error) {
    console.error("Lỗi tải giỏ hàng:", error);
  }
}
