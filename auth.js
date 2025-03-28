let isLoginMode = true;

function openAuthModal() {
  document.getElementById("auth-modal").classList.remove("hidden");
}

function closeAuthModal() {
  document.getElementById("auth-modal").classList.add("hidden");
}

function toggleAuthMode() {
  isLoginMode = !isLoginMode;
  document.getElementById("auth-modal-title").innerText = isLoginMode ? "Đăng nhập" : "Đăng ký";
  document.getElementById("auth-toggle-text").innerText = isLoginMode ? "Chưa có tài khoản?" : "Đã có tài khoản?";
  document.getElementById("auth-toggle-btn").innerText = isLoginMode ? "Đăng ký" : "Đăng nhập";

  // Ẩn/hiện trường đăng ký bổ sung nếu có
  const extraFields = document.querySelector(".register-extra");
  if (extraFields) {
    extraFields.classList.toggle("hidden", isLoginMode);
  }
}

function handleAuth() {
  const username = document.getElementById("auth-username").value;
  const password = document.getElementById("auth-password").value;

  if (!username || !password) {
    alert("Vui lòng nhập đầy đủ thông tin.");
    return;
  }

  if (isLoginMode) {
    const stored = JSON.parse(localStorage.getItem("user") || "{}");
    if (stored.username === username && stored.password === password) {
      alert("Đăng nhập thành công!");
      localStorage.setItem("loggedIn", "true");
      closeAuthModal();
      location.reload();
    } else {
      alert("Sai tên đăng nhập hoặc mật khẩu!");
    }
  } else {
    // Có thể mở rộng lưu thêm email và ngày sinh ở đây nếu muốn
    const email = document.getElementById("auth-email")?.value || "";
    const dob = document.getElementById("auth-dob")?.value || "";

    localStorage.setItem("user", JSON.stringify({ username, password, email, dob }));
    alert("Đăng ký thành công! Bạn có thể đăng nhập ngay.");
    toggleAuthMode();
  }
}

function checkLoginStatus() {
  const loggedIn = localStorage.getItem("loggedIn");
  const authDiv = document.querySelector(".auth-buttons");

  if (loggedIn === "true" && authDiv) {
    const user = JSON.parse(localStorage.getItem("user") || "{}");
    authDiv.innerHTML = `
      <span class="text-sm">Xin chào, ${user.username}</span>
      <button onclick="logout()" class="text-red-600 underline text-sm">Đăng xuất</button>
    `;
  }
}

function logout() {
  localStorage.removeItem("loggedIn");
  location.reload();
}

document.addEventListener("DOMContentLoaded", checkLoginStatus);
