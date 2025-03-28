// include-register.js
fetch("register-modal.html")
  .then(res => res.text())
  .then(html => document.body.insertAdjacentHTML("beforeend", html));
