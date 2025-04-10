const userId = sessionStorage.getItem("UserID");

document.addEventListener("DOMContentLoaded", () => {
  fetchOrders();
});

async function fetchOrders() {
  try {
    const res = await fetch(
      `http://localhost:5178/Order/GetOrdersByUserId/${userId}`
    );
    const orders = await res.json();

    const container = document.getElementById("order-list");
    if (!orders || orders.length === 0) {
      container.innerHTML = `<p class="text-gray-600">Chưa có đơn hàng nào.</p>`;
      return;
    }

    orders.forEach((order, index) => {
      const itemsHtml =
        order.items
          ?.map((item) => {
            const title = item.bookTitle || "Không rõ tên sách";
            const total =
              item.bookPrice != null
                ? Number(item.bookPrice).toLocaleString() + "đ"
                : "0đ";

            return `
          <li class="flex justify-between text-sm text-gray-700">
            <span>${title} (x${item.quantity})</span>
            <span>${total}</span>
          </li>`;
          })
          .join("") ??
        "<li class='text-sm text-gray-500'>Không có sản phẩm.</li>";

      const html = `
        <div class="border p-4 rounded-lg shadow-sm bg-white">
          <h2 class="text-xl font-semibold">Đơn hàng #${index + 1}</h2>
          <p><strong>Người nhận:</strong> ${order.customerName}</p>
          <p><strong>Địa chỉ:</strong> ${order.customerAddress}</p>
          <p><strong>Tổng tiền:</strong> ${
            order.totalAmount != null
              ? Number(order.totalAmount).toLocaleString() + "đ"
              : "0đ"
          }</p>
          <p><strong>Phương thức thanh toán:</strong> ${
            order.paymentMethod ?? "Thanh toán khi nhận hàng"
          }</p>

          <button class="text-blue-600 mt-2 underline" onclick="toggleDetails('details-${index}')">
            Xem chi tiết
          </button>
          <ul id="details-${index}" class="hidden mt-2 ml-4 list-disc text-sm text-gray-700">
            ${itemsHtml}
          </ul>
        </div>`;
      container.innerHTML += html;
    });
  } catch (err) {
    console.error("Lỗi khi lấy dữ liệu đơn hàng:", err);
    alert("Không thể tải dữ liệu đơn hàng.");
  }
}

function toggleDetails(id) {
  const el = document.getElementById(id);
  if (el) el.classList.toggle("hidden");
}
