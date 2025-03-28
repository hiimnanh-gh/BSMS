// login.js - xử lý sự kiện đăng nhập
document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("login-form");
  
    form.addEventListener("submit", function (e) {
      e.preventDefault();
      const username = form.username.value.trim();
      const password = form.password.value.trim();
  
      if (!username || !password) {
        alert("Vui lòng nhập đầy đủ thông tin.");
        return;
      }
  
      // Demo: sau này thay bằng gọi API
      console.log("Đăng nhập:", { username, password });
  
      alert("Đăng nhập thành công (demo)");
      // Ví dụ lưu session:
      localStorage.setItem("isLoggedIn", "true");
      localStorage.setItem("username", username);
      window.location.href = "/";
    });
  });
  