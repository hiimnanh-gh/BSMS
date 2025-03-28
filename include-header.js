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
    </nav>
  `;

  document.body.insertAdjacentHTML("afterbegin", headerHTML);

  renderAuthButtons();
  updateCartCount();
});

// -------------------------
// 👤 Login/Register hoặc Logout
function renderAuthButtons() {
  const authDiv = document.querySelector(".auth-buttons");
  const isLoggedIn = localStorage.getItem("loggedIn") === "true";
  const user = JSON.parse(localStorage.getItem("user") || "{}");

  if (!authDiv) return;

  if (isLoggedIn) {
    authDiv.innerHTML = `
      <span class="text-sm">👋 Xin chào, <strong>${user.username}</strong></span>
      <button onclick="logout()" class="text-red-600 underline text-sm">Đăng xuất</button>
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
    authDiv.innerHTML = `
      <button onclick="openModal()" class="bg-black text-white px-4 py-2 rounded hover:bg-gray-800 transition">Register</button>
      <a href="login.html">
        <button class="bg-black text-white px-4 py-2 rounded">Login</button>
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

  updateCartCount();
  setupCartPreview();
}

function logout() {
  localStorage.removeItem("loggedIn");
  localStorage.removeItem("user");
  localStorage.removeItem("cart");
  location.reload();
}

// -------------------------
// 🔁 Hiển thị số lượng + nội dung giỏ hàng
function updateCartCount() {
  const cart = JSON.parse(localStorage.getItem("cart") || "[]");
  const badge = document.getElementById("cart-count");
  if (badge) badge.textContent = cart.length;
}
// Hiển thị modal
function openAddModal() {
  document.getElementById("book-modal").classList.remove("hidden");
}

// Ẩn modal
function closeAddModal() {
  document.getElementById("book-modal").classList.add("hidden");
  document.getElementById("book-form").reset();
}

// Load dữ liệu sách từ localStorage
function loadBooks() {
  const books = JSON.parse(localStorage.getItem("books") || "[]");
  const tbody = document.getElementById("book-table-body");
  tbody.innerHTML = "";

  books.forEach((book, index) => {
    const tr = document.createElement("tr");
    tr.innerHTML = `
      <td class="py-2 px-4">${book.title}</td>
      <td class="py-2 px-4">${book.author}</td>
      <td class="py-2 px-4">${book.isbn}</td>
      <td class="py-2 px-4">${book.published}</td>
      <td class="py-2 px-4">${book.genre}</td>
      <td class="py-2 px-4">
        <button class="text-blue-600 hover:underline mr-2" onclick="editBook(${index})">Edit</button>
        <button class="text-red-600 hover:underline" onclick="deleteBook(${index})">Delete</button>
      </td>
    `;
    tbody.appendChild(tr);
  });
}

// Xoá sách
function deleteBook(index) {
  const books = JSON.parse(localStorage.getItem("books") || "[]");
  books.splice(index, 1);
  localStorage.setItem("books", JSON.stringify(books));
  loadBooks();
}
// Thêm sách mới
document.addEventListener("DOMContentLoaded", () => {
  const form = document.getElementById("book-form");
  if (form) {
    form.addEventListener("submit", (e) => {
      e.preventDefault();
      const formData = new FormData(form);
      const data = Object.fromEntries(formData.entries());
      const books = JSON.parse(localStorage.getItem("books") || "[]");

      if (form.image.files.length > 0) {
        const reader = new FileReader();
        reader.onload = function (e) {
          data.image = e.target.result;
          books.push(data);
          localStorage.setItem("books", JSON.stringify(books));
          closeAddModal();
          loadBooks();
        };
        reader.readAsDataURL(form.image.files[0]);
      } else {
        books.push(data);
        localStorage.setItem("books", JSON.stringify(books));
        closeAddModal();
        loadBooks();
      }
    });
  }

  loadBooks();
});
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
