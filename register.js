function openModal() {
    document.getElementById("register-modal").classList.remove("hidden");
  }
  
  function closeModal() {
    document.getElementById("register-modal").classList.add("hidden");
  }
  
  document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("register-form");
    if (!form) return;
  
    form.addEventListener("submit", (e) => {
      e.preventDefault();
  
      const data = {
        fullName: form.fullName.value,
        username: form.username.value,
        email: form.email.value,
        password: form.password.value,
        dob: form.dob.value,
      };
  
      const users = JSON.parse(localStorage.getItem("users") || "[]");
      users.push(data);
      localStorage.setItem("users", JSON.stringify(users));
  
      alert("Đăng ký thành công! Bạn có thể đăng nhập.");
      form.reset();
      closeModal();
    });
  });
  