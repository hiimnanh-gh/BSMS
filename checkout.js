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

  selectedBooks.forEach((book) => {
    const qty = parseInt(book.qty || 1);
    const price = parsePrice(book.price);
    total += price * qty;

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
  });

  total -= discount;
  if (total < 0) total = 0;
  totalEl.textContent = formatPrice(total);
}

function applyDiscount() {
  const code = document.getElementById("discount-code").value.toUpperCase();
  const validCodes = ["LOVEBOOK", "USAGI", "CHIIKAWA", "HACHIWARE"];

  if (validCodes.includes(code)) {
    discount = 15000;
    alert("Áp dụng mã giảm 15.000đ thành công!");
  } else {
    discount = 0;
    alert("Mã giảm giá không hợp lệ.");
  }

  renderCheckoutItems();
}

function updatePaymentFields() {
  const selected = document.querySelector(
    'input[name="payment"]:checked'
  ).value;
  const container = document.getElementById("payment-details");
  container.innerHTML = "";
  container.classList.toggle("hidden", false);

  if (selected === "credit" || selected === "bank") {
    container.innerHTML = `
      <input type="text" placeholder="Tên chủ thẻ" class="w-full border px-4 py-2 rounded mb-3" />
      <input type="text" placeholder="Số thẻ" class="w-full border px-4 py-2 rounded mb-3" />
      <input type="text" placeholder="Ngày hết hạn (MM/YY)" class="w-full border px-4 py-2 rounded mb-3" />
    `;
  } else if (selected === "transfer") {
    container.innerHTML = `
      <p class="text-sm mb-2 text-gray-700">Quét mã QR để chuyển khoản:</p>
      <img src="QR.jpg" alt="QR Code" class="w-40 h-40 object-contain" />
    `;
  } else {
    container.classList.add("hidden");
  }
}

function submitOrder() {
  const name = document.getElementById("customer-name").value;
  const phone = document.getElementById("customer-phone").value;
  const address = document.getElementById("customer-address").value;
  const totalAmount = document.getElementById("total-amount").textContent;
  const paymentMethod = document.querySelector(
    'input[name="payment"]:checked'
  ).value;

  if (!name || !phone || !address) {
    alert("Vui lòng điền đầy đủ thông tin người nhận.");
    return;
  }

  // Tạo đơn hàng mới
  const order = {
    name,
    phone,
    address,
    totalAmount,
    paymentMethod,
  };

  const selectedBooks = JSON.parse(localStorage.getItem("checkout") || "[]");
  let discount = 0;

  function formatPrice(num) {
    return num.toLocaleString("vi-VN") + "đ";
  }

  // Xóa giỏ hàng sau khi đặt hàng
  localStorage.removeItem("checkout");
  localStorage.removeItem("cart");

  // Chuyển hướng sang trang đặt hàng thành công
  window.location.href = "success.html";
}

// Gọi khi load
renderCheckoutItems();
document
  .querySelectorAll('input[name="payment"]')
  .forEach((el) => el.addEventListener("change", updatePaymentFields));

function parsePrice(priceStr) {
  return parseInt((priceStr || "0").replace(/[^\d]/g, ""));
}

function renderCheckoutItems() {
  const container = document.getElementById("checkout-items");
  const totalEl = document.getElementById("total-amount");
  container.innerHTML = "";
  let total = 0;

  selectedBooks.forEach((book) => {
    const qty = parseInt(book.qty || 1);
    const price = parsePrice(book.price);
    total += price * qty;

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
  });

  total -= discount;
  if (total < 0) total = 0;
  totalEl.textContent = formatPrice(total);
}

function applyDiscount() {
  const code = document.getElementById("discount-code").value.toUpperCase();
  const validCodes = ["LOVEBOOK", "USAGI", "CHIIKAWA", "HACHIWARE"];

  if (validCodes.includes(code)) {
    discount = 15000;
    alert("Áp dụng mã giảm 15.000đ thành công!");
  } else {
    discount = 0;
    alert("Mã giảm giá không hợp lệ.");
  }

  renderCheckoutItems();
}

function updatePaymentFields() {
  const selected = document.querySelector(
    'input[name="payment"]:checked'
  ).value;
  const container = document.getElementById("payment-details");
  container.innerHTML = "";
  container.classList.toggle("hidden", false);

  if (selected === "credit" || selected === "bank") {
    container.innerHTML = `
      <input type="text" placeholder="Tên chủ thẻ" class="w-full border px-4 py-2 rounded mb-3" />
      <input type="text" placeholder="Số thẻ" class="w-full border px-4 py-2 rounded mb-3" />
      <input type="text" placeholder="Ngày hết hạn (MM/YY)" class="w-full border px-4 py-2 rounded mb-3" />
    `;
  } else if (selected === "transfer") {
    container.innerHTML = `
      <p class="text-sm mb-2 text-gray-700">Quét mã QR để chuyển khoản:</p>
      <img src="QR.jpg" alt="QR Code" class="w-40 h-40 object-contain" />
    `;
  } else {
    container.classList.add("hidden");
  }
}

function submitOrder() {
  const name = document.getElementById("customer-name").value;
  const phone = document.getElementById("customer-phone").value;
  const address = document.getElementById("customer-address").value;
  const totalAmount = document.getElementById("total-amount").textContent;
  const paymentMethod = document.querySelector(
    'input[name="payment"]:checked'
  ).value;

  if (!name || !phone || !address) {
    alert("Vui lòng điền đầy đủ thông tin người nhận.");
    return;
  }

  // Tạo đơn hàng mới
  const order = {
    name,
    phone,
    address,
    totalAmount,
    paymentMethod,
  };

  // Lưu đơn hàng vào LocalStorage
  let orders = JSON.parse(localStorage.getItem("orders")) || [];
  orders.push(order);
  localStorage.setItem("orders", JSON.stringify(orders));

  // Xóa giỏ hàng sau khi đặt hàng
  localStorage.removeItem("checkout");
  localStorage.removeItem("cart");

  // Chuyển hướng sang trang đặt hàng thành công
  window.location.href = "success.html";
}

// Gọi khi load
renderCheckoutItems();
document
  .querySelectorAll('input[name="payment"]')
  .forEach((el) => el.addEventListener("change", updatePaymentFields));
