document.addEventListener("DOMContentLoaded", () => {
  const headerHTML = `
    <!-- HEADER -->
    <header class="flex items-center justify-between px-6 py-4 bg-white shadow">
      <div class="flex items-center gap-3">
        <a href="/">
          <img src="./images(1).jpg" alt="logo" width="40" class="h-10 w-10 rounded-full object-cover" />
        </a>
        <input
          type="text"
          placeholder="Search books, authors, ISBNs"
          class="border rounded-md px-4 py-2 w-80 shadow-sm"
        />
      </div>
      <div class="flex items-center gap-3 auth-buttons">
        <!-- Auth buttons sẽ được render bằng JavaScript -->
      </div>
    </header>

    <!-- NAVBAR -->
    <nav class="flex gap-6 px-6 py-3 border-b bg-gray-100">
      <a href="index.html" class="hover:underline">Home</a>
      <a href="book.html" class="hover:underline">Books</a>
      <a href="book-management.html" class="hover:underline">Book Management</a>
      <a href="orders.html" class="hover:underline">Orders</a>
    </nav>
  `;

  document.body.insertAdjacentHTML("afterbegin", headerHTML);

  renderAuthButtons();
  updateCartCount();
});

// -------------------------
// 👤 Login/Register hoặc Logout
async function renderAuthButtons() {
  const authDiv = document.querySelector(".auth-buttons");
  const userId = sessionStorage.getItem("UserID");
  const username = sessionStorage.getItem("UserName");

  if (!authDiv) return;

  if (userId && username) {
    authDiv.innerHTML = `
      <span class="text-sm">👋 Xin chào, <strong>${username}</strong></span>
      <button onclick="logout()" class="text-red-600 underline text-sm ml-2">Đăng xuất</button>
      <div class="relative group cursor-pointer">
        <svg
          xmlns="http://www.w3.org/2000/svg"
          class="w-6 h-6"
          fill="none"
          viewBox="0 0 24 24"
          stroke="currentColor"
          onclick="window.location.href='cart.html'"
        >
          <path d="M3 3h2l.4 2M7 13h10l4-8H5.4" stroke-width="2" />
          <circle cx="9" cy="21" r="1" />
          <circle cx="20" cy="21" r="1" />
        </svg>
        <span id="cart-count" class="absolute -top-2 -right-2 bg-red-500 text-white text-xs rounded-full px-1">0</span>

        <!-- Hover Popover -->
        <div id="cart-preview" class="absolute right-0 mt-2 w-64 bg-white shadow-lg rounded-md p-3 hidden group-hover:block z-50 text-sm">
          <div id="cart-items-preview">Đang tải...</div>
        </div>
      </div>
    `;
  } else {
    showLoginButtons();
  }

  updateCartCount();
  setupCartPreview();
}

// Hiển thị nút đăng ký & đăng nhập khi chưa có user
function showLoginButtons() {
  const authDiv = document.querySelector(".auth-buttons");
  authDiv.innerHTML = `
    <a href="register-modal.html">
    <button class="bg-black text-white px-4 py-2 rounded ml-2">Register</button>
    </a>
    <a href="login.html">
      <button class="bg-black text-white px-4 py-2 rounded ml-2">Login</button>
    </a>
    <div class="relative group cursor-pointer">
      <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6 text-black" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path d="M3 3h2l.4 2M7 13h10l4-8H5.4" stroke-width="2" />
        <circle cx="9" cy="21" r="1" />
        <circle cx="20" cy="21" r="1" />
      </svg>
      <span id="cart-count" class="absolute -top-2 -right-2 bg-red-500 text-white text-xs rounded-full px-1">0</span>

      <!-- Hover Popover -->
      <div id="cart-preview" class="absolute right-0 mt-2 w-64 bg-white shadow-lg rounded-md p-3 hidden group-hover:block z-50 text-sm">
        <div id="cart-items-preview">Đang tải...</div>
      </div>
    </div>
  `;
}

// Xử lý logout: Xóa sessionStorage, reload trang
function logout() {
  console.log("Đang logout..."); // Kiểm tra xem sự kiện có chạy không
  sessionStorage.clear(); // Xóa toàn bộ sessionStorage
  window.location.href = "login.html"; // Chuyển về trang đăng nhập sau khi logout
}

// -------------------------
// 🔁 Hiển thị số lượng + nội dung giỏ hàng
function updateCartCount() {
  const cart = JSON.parse(localStorage.getItem("cart") || "[]");
  const badge = document.getElementById("cart-count");
  if (badge) badge.textContent = cart.length;
}

// 🎯 Preview giỏ hàng mỗi khi hover
function setupCartPreview() {
  const previewContainer = document.getElementById("cart-items-preview");
  if (!previewContainer) return;

  const render = () => {
    const cart = JSON.parse(localStorage.getItem("cart") || "[]");
    if (cart.length === 0) {
      previewContainer.innerHTML = `<p class="text-gray-500 text-sm">Giỏ hàng trống.</p>`;
    } else {
      previewContainer.innerHTML = cart
        .map((item, i) => `<div class="mb-1">📘 ${item.title}</div>`)
        .join("");
    }
  };

  document.querySelector(".group")?.addEventListener("mouseenter", render);
}

// Gọi renderAuthButtons() khi load trang
document.addEventListener("DOMContentLoaded", renderAuthButtons);
