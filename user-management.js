document.addEventListener("DOMContentLoaded", () => {
  fetch("http://localhost:5178/User/GetAll")
    .then((res) => {
      if (!res.ok) throw new Error("Không thể lấy dữ liệu user.");
      return res.json();
    })
    .then((users) => {
      const customerUsers = users.filter((user) => user.role === "customer");
      const tbody = document.getElementById("userTableBody");

      if (customerUsers.length === 0) {
        tbody.innerHTML = `<tr><td colspan="2" class="text-center py-4 text-gray-500">Không có người dùng customer nào.</td></tr>`;
        return;
      }

      customerUsers.forEach((user) => {
        const row = document.createElement("tr");
        row.innerHTML = `
              <td class="px-4 py-2">${user.username}</td>
              <td class="px-4 py-2">${user.email}</td>
            `;
        tbody.appendChild(row);
      });
    })
    .catch((err) => {
      console.error("Lỗi khi lấy danh sách user:", err);
    });
});
