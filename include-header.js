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
    <nav class="flex gap-6 px-6 py-3 border-b bg-gray-100" id="navbar">
      <a href="index.html" class="hover:underline">Home</a>
      <a href="book.html" class="hover:underline">Books</a>
      <!-- Book Management và Orders sẽ được thêm hoặc ẩn dựa trên vai trò -->
    </nav>
  `;

  // Insert the header and navbar into the body
  document.body.insertAdjacentHTML("afterbegin", headerHTML);

  // Phân quyền và render navbar
  const role = sessionStorage.getItem("role"); // Lấy vai trò người dùng từ sessionStorage
  const navbar = document.getElementById("navbar");

  if (role === "admin") {
    // Nếu là admin, hiển thị Book Management
    const bookManagementLink = document.createElement("a");
    bookManagementLink.href = "book-management.html";
    bookManagementLink.classList.add("hover:underline");
    bookManagementLink.textContent = "Book Management";
    navbar.appendChild(bookManagementLink);
  } else if (role === "customer") {
    // Nếu là customer, hiển thị Orders
    const ordersLink = document.createElement("a");
    ordersLink.href = "orders.html";
    ordersLink.classList.add("hover:underline");
    ordersLink.textContent = "Orders";
    navbar.appendChild(ordersLink);
  }

  // Render các nút Auth
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
// 🛒 Lấy dữ liệu giỏ hàng từ API
async function acb() { 
  try {
    const cartID = sessionStorage.getItem("cartID");

    const response = await fetch(`${API_URL}/GetList?cartID=${cartID}`);

    if (!response.ok) throw new Error("Không thể lấy giỏ hàng");

    const cartItems = await response.json();

    // Lọc chỉ lấy tên sách và số lượng
    const cartSummary = cartItems.map((item) => ({
      bookTitle: item.bookTitle,
      quantity: item.quantity,
    }));

    console.log("Giỏ hàng (Chỉ tên sách và số lượng):", cartSummary); // Debug kiểm tra
    renderCart(cartSummary); // Gọi hàm renderCart với dữ liệu đã lọc
  } catch (error) {
    console.error("Lỗi tải giỏ hàng:", error);
  }
}

// 🎨 Render giỏ hàng chỉ với tên sách và số lượng
function renderCart(cartItems) {
  const container = document.getElementById("cart-list");
  container.innerHTML = "";

  if (!cartItems || cartItems.length === 0) {
    container.innerHTML = `<p class="text-gray-600">Giỏ hàng trống.</p>`;
    return;
  }

  cartItems.forEach((item) => {
    const div = document.createElement("div");
    div.className =
      "cart-item flex items-center justify-between bg-white p-4 rounded shadow";

    div.innerHTML = `
      <div class="flex items-center gap-3">
        <div>
          <p class="font-semibold">${item.bookTitle}</p>
          <p class="text-sm text-gray-500">Số lượng: ${item.quantity}</p>
        </div>
      </div>
    `;

    container.appendChild(div);
  });
}

// 🔁 Hiển thị số lượng + nội dung giỏ hàng
function updateCartCount() {
  const cartID = sessionStorage.getItem("cartID");

  fetch(`${API_URL}/GetList?cartID=${cartID}`)
    .then((response) => response.json())
    .then((cartItems) => {
      const badge = document.getElementById("cart-count");
      if (badge) badge.textContent = cartItems.length;
    })
    .catch((error) => console.error("Lỗi cập nhật số lượng giỏ hàng:", error));
}

// 🎯 Preview giỏ hàng mỗi khi hover
function setupCartPreview() {
  const previewContainer = document.getElementById("cart-items-preview");
  if (!previewContainer) return;

  const render = () => {
    const cartID = sessionStorage.getItem("cartID");

    fetch(`${API_URL}/GetList?cartID=${cartID}`)
      .then((response) => response.json())
      .then((cartItems) => {
        if (cartItems.length === 0) {
          previewContainer.innerHTML = `<p class="text-gray-500 text-sm">Giỏ hàng trống.</p>`;
        } else {
          previewContainer.innerHTML = cartItems
            .map(
              (item) =>
                `<div class="mb-1">📘 ${item.bookTitle} (Số lượng: ${item.quantity})</div>`
            )
            .join("");
        }
      })
      .catch((error) =>
        console.error("Lỗi khi hover preview giỏ hàng:", error)
      );
  };

  document.querySelector(".group")?.addEventListener("mouseenter", render);
}

// Gọi renderAuthButtons() khi load trang
document.addEventListener("DOMContentLoaded", renderAuthButtons);
