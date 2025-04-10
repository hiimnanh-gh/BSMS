const API_URL = "http://localhost:5178";

document.addEventListener("DOMContentLoaded", () => {
  const userId = sessionStorage.getItem("UserID");
  if (!userId) {
    alert("Bạn cần đăng nhập để xem đơn hàng.");
    window.location.href = "login.html";
    return;
  }

  fetch(`${API_URL}/Order/GetOrdersByUserId/${userId}`)
    .then((res) => res.json())
    .then((orders) => renderTracking(orders))
    .catch((err) => {
      console.error("Lỗi khi lấy đơn hàng:", err);
      alert("Không thể tải đơn hàng.");
    });
});

function renderTracking(orders) {
  const container = document.getElementById("tracking-container");
  if (!orders || orders.length === 0) {
    container.innerHTML = `<p class="text-gray-600">Không có đơn hàng nào.</p>`;
    return;
  }

  orders.forEach((order, index) => {
    const div = document.createElement("div");
    div.className = "bg-white p-4 rounded shadow";

    div.innerHTML = `
      <h2 class="text-lg font-semibold">Đơn hàng #${index + 1}</h2>
      <p><strong>Người nhận:</strong> ${order.customerName}</p>
      <p><strong>Địa chỉ:</strong> ${order.customerAddress}</p>
      <p><strong>Trạng thái:</strong> ${order.status}</p>
      <button
        class="text-blue-600 underline mt-2"
        onclick="toggleMap('${order.customerAddress}', 'map-${index}')"
      >
        Xem vị trí
      </button>
      <div id="map-${index}" class="mt-3 h-[300px] rounded hidden"></div>
    `;

    container.appendChild(div);
  });
}

function toggleMap(address, mapId) {
  const mapDiv = document.getElementById(mapId);

  if (!mapDiv.classList.contains("hidden")) {
    mapDiv.classList.add("hidden");
    mapDiv.innerHTML = ""; // Xóa nội dung bản đồ khi ẩn
    return;
  }

  mapDiv.classList.remove("hidden");

  // Dùng API free để geocode địa chỉ
  fetch(
    `https://nominatim.openstreetmap.org/search?format=json&q=${encodeURIComponent(
      address
    )}`
  )
    .then((res) => res.json())
    .then((data) => {
      if (!data || data.length === 0) {
        mapDiv.innerHTML = "<p class='text-red-500'>Không tìm thấy vị trí.</p>";
        return;
      }

      const { lat, lon } = data[0];

      // Xóa nội dung cũ trước khi tạo bản đồ mới
      mapDiv.innerHTML = "";

      const map = L.map(mapId).setView([lat, lon], 16);

      L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
        attribution:
          '&copy; <a href="https://www.openstreetmap.org/">OpenStreetMap</a>',
      }).addTo(map);

      L.marker([lat, lon]).addTo(map).bindPopup("Vị trí giao hàng").openPopup();
    })
    .catch((err) => {
      console.error("Lỗi geocoding:", err);
      mapDiv.innerHTML = "<p class='text-red-500'>Lỗi khi tải bản đồ.</p>";
    });
}
