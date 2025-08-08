let email = "";

window.onload = function () {
  const params = new URLSearchParams(window.location.search);
  email = params.get("email");
  document.getElementById("userEmail").innerText = email;
};

function verifyOtp() {
  const otp = document.getElementById("otp").value.trim();

  if (!otp) {
    alert("Enter OTP");
    return;
  }

  fetch("http://localhost:5038/api/Otp/Verify", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email: email, otp: parseInt(otp) })
  })
  .then(res => {
    if (!res.ok) throw new Error("Invalid or expired OTP.");
    return res.json();
  })
  .then(data => {
    alert("Login successful!");
    window.location.href = "dashboard.html";
  })
  .catch(err => {
    alert(err.message);
  });
}

function regenerateOtp() {
  fetch("http://localhost:5038/api/Otp/Generate", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email: email })
  })
  .then(res => res.json())
  .then(data => {
    alert(data.message || "OTP regenerated.");
  })
  .catch(err => {
    alert("Failed to regenerate OTP.");
  });
}