document.addEventListener("DOMContentLoaded", () => {
  const API_URL = "http://localhost:5178";

  const headerHTML = `
    <header class="flex items-center justify-between px-6 py-4 bg-white shadow">
      <div class="flex items-center gap-3">
        <a href="/">
          <img src="./images(1).jpg" alt="logo" width="40" class="h-10 w-10 rounded-full object-cover" />
        </a>
        <div class="relative">
          <input id="searchInput" type="text" placeholder="Tìm sách..." class="border px-4 py-2 rounded w-80" />
          <ul id="searchResults" class="absolute left-0 top-full bg-white border border-gray-200 rounded shadow-md hidden w-full mt-1 max-h-60 overflow-auto z-50"></ul>
        </div>
      </div>
      <div class="flex items-center gap-3 auth-buttons"></div>
    </header> 

    <nav class="flex gap-6 px-6 py-3 border-b bg-gray-100" id="navbar">
      <a href="index.html" class="hover:underline">Trang Chủ</a>
      <a href="book.html" class="hover:underline">Sách</a>
    </nav>
  `;

  document.body.insertAdjacentHTML("afterbegin", headerHTML);

  const role = sessionStorage.getItem("role");
  const navbar = document.getElementById("navbar");

  if (role === "admin") {
    const bookManagement = createNavLink(
      "book-management.html",
      "Quản lý sách"
    );
    const userManagement = createNavLink(
      "user-management.html",
      "Quản lý người dùng"
    );
    const orders = createNavLink("orders.html", "Đơn hàng");
    navbar.append(bookManagement, userManagement, orders);
  } else if (role === "customer") {
    const orders = createNavLink("orders.html", "đơn hàng");
    navbar.appendChild(orders);
  }

  renderAuthButtons();
  updateCartCount(API_URL);
  setupCartPreview(API_URL);
});

function createNavLink(href, text) {
  const link = document.createElement("a");
  link.href = href;
  link.className = "hover:underline";
  link.textContent = text;
  return link;
}

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
        <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor" onclick="window.location.href='cart.html'">
          <path d="M3 3h2l.4 2M7 13h10l4-8H5.4" stroke-width="2" />
          <circle cx="9" cy="21" r="1" />
          <circle cx="20" cy="21" r="1" />
        </svg>
        <span id="cart-count" class="absolute -top-2 -right-2 bg-red-500 text-white text-xs rounded-full px-1">0</span>
        <div id="cart-preview" class="absolute right-0 mt-2 w-64 bg-white shadow-lg rounded-md p-3 hidden group-hover:block z-50 text-sm">
          <div id="cart-items-preview">Đang tải...</div>
        </div>
      </div>
    `;
  } else {
    showLoginButtons();
  }
}

function showLoginButtons() {
  const authDiv = document.querySelector(".auth-buttons");
  authDiv.innerHTML = `
    <a href="register-modal.html"><button class="bg-black text-white px-4 py-2 rounded ml-2">Register</button></a>
    <a href="login.html"><button class="bg-black text-white px-4 py-2 rounded ml-2">Login</button></a>
  `;
}

function logout() {
  sessionStorage.clear();
  window.location.href = "login.html";
}

function updateCartCount(API_URL) {
  const cartID = sessionStorage.getItem("cartID");
  fetch(`${API_URL}/Cart/GetList?cartID=${cartID}`)
    .then((res) => res.json())
    .then((items) => {
      const badge = document.getElementById("cart-count");
      if (badge) badge.textContent = items.length;
    })
    .catch((err) => console.error("Lỗi cập nhật số lượng giỏ hàng:", err));
}

function setupCartPreview(API_URL) {
  const preview = document.getElementById("cart-items-preview");
  if (!preview) return;

  const render = () => {
    const cartID = sessionStorage.getItem("cartID");
    fetch(`${API_URL}/GetList?cartID=${cartID}`)
      .then((res) => res.json())
      .then((items) => {
        preview.innerHTML =
          items.length === 0
            ? "<p class='text-gray-500 text-sm'>Giỏ hàng trống.</p>"
            : items
                .map(
                  (item) =>
                    `<div class="mb-1">📘 ${item.bookTitle} (Số lượng: ${item.quantity})</div>`
                )
                .join("");
      })
      .catch((err) => console.error("Lỗi preview giỏ hàng:", err));
  };

  document.querySelector(".group")?.addEventListener("mouseenter", render);
}
