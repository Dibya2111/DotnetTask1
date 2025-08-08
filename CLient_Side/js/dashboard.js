const apiBase = "http://localhost:5038/api/Employees";

window.onload = loadEmployees;

function loadEmployees() {
  fetch(apiBase)
    .then(res => res.json())
    .then(data => {
      const table = document.querySelector("#employeeTable tbody");
      table.innerHTML = "";
      data.forEach(emp => {
        const row = `<tr>
          <td>${emp.name}</td>
          <td>${emp.email}</td>
          <td>${emp.phoneNumber}</td>
          <td>${emp.salary}</td>
          <td>${emp.dateOfJoining.split("T")[0]}</td>
          <td>${emp.isActive}</td>
          <td>
            <button class="action-btn update-btn" onclick='editEmployee(${JSON.stringify(emp)})'>Update</button>
            <button class="action-btn delete-btn" onclick='deleteEmployee(${emp.id})'>Delete</button>
          </td>
        </tr>`;
        table.innerHTML += row;
      });
    });
}

function addEmployee() {
  const newEmp = {
    name: document.getElementById("name").value,
    email: document.getElementById("empEmail").value,
    phoneNumber: document.getElementById("phone").value,
    salary: parseFloat(document.getElementById("salary").value),
    dateOfJoining: document.getElementById("doj").value,
    isActive: true
  };

  fetch(apiBase, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(newEmp)
  })
    .then(res => {
      if (res.ok) {
        alert("Employee added!");
        loadEmployees();
      } else {
        alert("Failed to add employee.");
      }
    });
}

let currentEmployeeId = null;

function editEmployee(emp) {
  currentEmployeeId = emp.id;
  document.getElementById("updateName").value = emp.name;
  document.getElementById("updateEmail").value = emp.email;
  document.getElementById("updatePhone").value = emp.phoneNumber;
  document.getElementById("updateSalary").value = emp.salary;
  document.getElementById("updateDoj").value = emp.dateOfJoining.split("T")[0];
  
  document.getElementById("updateModal").style.display = "flex";
}

function updateEmployee() {
  const updatedEmp = {
    id: currentEmployeeId,
    name: document.getElementById("updateName").value,
    email: document.getElementById("updateEmail").value,
    phoneNumber: document.getElementById("updatePhone").value,
    salary: parseFloat(document.getElementById("updateSalary").value),
    dateOfJoining: document.getElementById("updateDoj").value,
    isActive: true
  };

  fetch(`${apiBase}/${currentEmployeeId}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(updatedEmp)
  })
    .then(res => {
      if (res.ok) {
        closeModal();
        loadEmployees();
      } else {
        alert("Update failed.");
      }
    });
}

function closeModal() {
  document.getElementById("updateModal").style.display = "none";
  currentEmployeeId = null;
}

function deleteEmployee(id) {
  fetch(`${apiBase}/${id}`, { method: "DELETE" })
    .then(res => {
      if (res.ok) {
        alert("Deleted!");
        loadEmployees();
      } else {
        alert("Delete failed.");
      }
    });
}