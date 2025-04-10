document.addEventListener("DOMContentLoaded", () => {
  const searchInput = document.getElementById("searchInput");
  const searchResults = document.getElementById("searchResults");

  if (!searchInput || !searchResults) return;

  let books = [];

  fetch("http://localhost:5178/Book/GetList")
    .then((res) => res.json())
    .then((data) => {
      books = data;
    })
    .catch((err) => {
      console.error("Không thể tải danh sách sách:", err);
    });

  searchInput.addEventListener("input", () => {
    const keyword = searchInput.value.trim().toLowerCase();
    searchResults.innerHTML = "";

    if (keyword === "") {
      searchResults.classList.add("hidden");
      return;
    }

    const results = books.filter((book) =>
      book.title.toLowerCase().includes(keyword)
    );

    if (results.length === 0) {
      searchResults.innerHTML = `
        <li class="px-4 py-2 text-gray-500">Không tìm thấy kết quả</li>`;
    } else {
      results.forEach((book) => {
        const li = document.createElement("li");
        li.className =
          "px-4 py-2 hover:bg-gray-100 cursor-pointer text-sm border-b";
        li.innerHTML = `<span class="text-gray-800">${book.title}</span>`;

        li.addEventListener("click", () => {
          sessionStorage.setItem("bookId", book.bookId); // Lưu lại bookId
          window.location.href = "book-detail.html"; // Chuyển trang
        });

        searchResults.appendChild(li);
      });
    }

    searchResults.classList.remove("hidden");
  });

  // Ẩn dropdown khi click ra ngoài
  document.addEventListener("click", (e) => {
    if (!searchInput.contains(e.target) && !searchResults.contains(e.target)) {
      searchResults.classList.add("hidden");
    }
  });
});
