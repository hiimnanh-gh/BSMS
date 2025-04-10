document.addEventListener("DOMContentLoaded", () => {
  const form = document.getElementById("register-form");
  if (!form) return;

  form.addEventListener("submit", async (e) => {
    e.preventDefault();

    const data = {
      fullName: form.fullName.value,
      username: form.username.value,
      email: form.email.value,
      password: form.password.value,
      dob: form.dob.value,
      role: "customer", // gán ngầm
    };

    try {
      // Gọi API đăng ký
      const response = await fetch("https://yourapiurl.com/user/register", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(data),
      });

      const result = await response.json();
      if (response.ok) {
        alert(result.message); // Đăng ký thành công
        form.reset();
        closeModal();
      } else {
        alert(result.message); // Thông báo lỗi từ API
      }
    } catch (error) {
      console.error("Đã có lỗi xảy ra", error);
      alert("Đăng ký không thành công, vui lòng thử lại.");
    }
  });
});
