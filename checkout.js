const selectedBooks = JSON.parse(localStorage.getItem("checkout") || "[]");
let discount = 0;

function formatPrice(num) {
  return num.toLocaleString("vi-VN") + "đ";
}

function parsePrice(priceStr) {
  return parseInt((priceStr || "0").replace(/[^\d]/g, ""));
}

function renderCheckoutItems() {
  const container = document.getElementById("checkout-items");
  const totalEl = document.getElementById("total-amount");
  container.innerHTML = "";
  let total = 0;

  selectedBooks.forEach(book => {
    const qty = parseInt(book.qty || 1);
    const price = parsePrice(book.price);

    const div = document.createElement("div");
    div.className = "flex justify-between items-center border-b pb-2";

    div.innerHTML = `
      <div>
        <h3 class="text-lg font-semibold">${book.title}</h3>
        <p class="text-sm text-gray-500">Giá: ${formatPrice(price)} × ${qty}</p>
      </div>
      <p class="font-bold">${formatPrice(price * qty)}</p>
    `;

    container.appendChild(div);
    total += price * qty;
  });

  total = total - discount;
  if (total < 0) total = 0;

  totalEl.textContent = formatPrice(total);
}

function applyDiscount() {
  const code = document.getElementById("discount-code").value;
  const validCodes = ["LOVEBOOK", "USAGI", "CHIIKAWA", "HACHIWARE"];

  if (validCodes.includes(code)) {
    discount = 15000;
    alert("Áp dụng mã giảm giá thành công!");
  } else {
    discount = 0;
    alert("Mã giảm giá không hợp lệ.");
  }

  renderCheckoutItems();
}

function submitOrder() {
  const name = document.getElementById("customer-name").value.trim();
  const phone = document.getElementById("customer-phone").value.trim();
  const address = document.getElementById("customer-address").value.trim();
  const payment = document.querySelector('input[name="payment"]:checked')?.value || "cod";

  if (!name || !phone || !address) {
    alert("Vui lòng nhập đầy đủ thông tin người nhận.");
    return;
  }

  const order = {
    items: selectedBooks,
    total: document.getElementById("total-amount").textContent,
    customer: { name, phone, address },
    payment,
    discount,
    status: "Đang xử lý",
    createdAt: new Date().toISOString()
  };

  localStorage.setItem("currentOrder", JSON.stringify(order));
  localStorage.removeItem("checkout");
  localStorage.removeItem("cart");

  alert("🎉 Đặt hàng thành công!");
  window.location.href = "order-tracking.html";
}

// Xử lý thông tin thanh toán bổ sung
document.querySelectorAll('input[name="payment"]').forEach(radio => {
  radio.addEventListener("change", () => {
    const method = radio.value;
    const container = document.getElementById("payment-details");

    if (method === "credit" || method === "bank") {
      container.innerHTML = `
        <input type="text" placeholder="Tên chủ thẻ" class="w-full border px-4 py-2 rounded mb-2" />
        <input type="text" placeholder="Số thẻ" class="w-full border px-4 py-2 rounded mb-2" />
        <input type="text" placeholder="Ngày hết hạn (MM/YY)" class="w-full border px-4 py-2 rounded mb-2" />
      `;
      container.classList.remove("hidden");
    } else if (method === "transfer") {
      container.innerHTML = `
        <p class="mb-2">Vui lòng chuyển khoản đến tài khoản sau và đính kèm mã đơn hàng:</p>
        <img src="QR.jpg" alt="QR chuyển khoản" class="w-40 h-40 mx-auto rounded border" />
      `;
      container.classList.remove("hidden");
    } else {
      container.innerHTML = "";
      container.classList.add("hidden");
    }
  });
});

// Khởi tạo trang
renderCheckoutItems();
