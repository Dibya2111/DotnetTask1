function registerUser() {
  const username = document.getElementById("username").value.trim();
  const email = document.getElementById("email").value.trim();

  if (!username || !email) {
    showMessage("Please fill all fields.", "error");
    return;
  }
  
  fetch("http://localhost:5038/api/User", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ username: username, userEmail: email })
  })
  .then(response => {
    if (!response.ok) {
      showUserExistsMessage(email);
      return;
    }
    return generateOtpAndRedirect(email);
  })
  .catch(error => {
    console.error("Error:", error);
    showMessage("Something went wrong.", "error");
  });
}

function generateOtpAndRedirect(email) {
  return fetch("http://localhost:5038/api/Otp/Generate", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email: email })
  })
  .then(res => res.json())
  .then(data => {
    if (data.message) {
      showMessage(data.message, "success");
    }
    setTimeout(() => {
      window.location.href = `otp.html?email=${encodeURIComponent(email)}`;
    }, 1500);
  });
}

function showUserExistsMessage(email) {
  const container = document.querySelector('.registration-container');
  const existingMsg = container.querySelector('.user-exists-message');
  if (existingMsg) existingMsg.remove();
  
  const messageDiv = document.createElement('div');
  messageDiv.className = 'user-exists-message';
  messageDiv.innerHTML = `
    <div class="error">User already exists with this email!</div>
    <button onclick="generateOtpAndRedirect('${email}')" class="otp-redirect-btn">Generate OTP & Login</button>
  `;
  
  container.insertBefore(messageDiv, container.querySelector('button'));
}

function showMessage(message, type) {
  const container = document.querySelector('.registration-container');
  const existingMsg = container.querySelector('.message');
  if (existingMsg) existingMsg.remove();
  
  const messageDiv = document.createElement('div');
  messageDiv.className = `message ${type}`;
  messageDiv.textContent = message;
  
  container.insertBefore(messageDiv, container.querySelector('.input-group'));
  
  setTimeout(() => messageDiv.remove(), 3000);
}
