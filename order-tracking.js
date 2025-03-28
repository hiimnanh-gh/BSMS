document.addEventListener("DOMContentLoaded", () => {
    // Hà Nội giả định là vị trí giao
    const lat = 21.0285;
    const lng = 105.8542;
  
    const map = L.map('map').setView([lat, lng], 13);
  
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '© OpenStreetMap contributors'
    }).addTo(map);
  
    L.marker([lat, lng])
      .addTo(map)
      .bindPopup("📦 Đơn hàng của bạn đang ở đây!")
      .openPopup();
  });
  