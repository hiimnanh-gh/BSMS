const users = [
    { username: "admin", password: "123456", fullName: "Quản trị viên" },
    { username: "user1", password: "password", fullName: "Người dùng 1" },
  ];
  
  document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("login-form");
  
    form.addEventListener("submit", function (e) {
      e.preventDefault();
      const username = form.username.value.trim();
      const password = form.password.value.trim();
  
      const user = users.find(
        (u) => u.username === username && u.password === password
      );
  
      if (!user) {
        alert("Tên đăng nhập hoặc mật khẩu sai!");
        return;
      }
  
      alert(`Xin chào, ${user.fullName}`);
      localStorage.setItem("isLoggedIn", "true");
      localStorage.setItem("username", user.username);
      window.location.href = "/";
    });
  });
  