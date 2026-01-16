import { useState } from "react";
import api from "../api/httpClient";
import "./Login.css";
import { Navigate } from "react-router-dom";
import { useNavigate } from "react-router-dom";


export default function CreateAdmin() {
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const navigate = useNavigate();


  const handleCreateAdmin = async () => {
    if (password !== confirmPassword) {
      alert("Passwords do not match");
      return;
    }

    try {
      await api.post("/auth/create-admin", {
        firstName,
        lastName,
        email,
        password,
      });

      alert("Admin created successfully");

   
      navigate("/dashboard");

    } catch (error) {
      alert(error.response?.data || "Admin creation failed");
    }
  };


  return (
    <div className="login-page">
      <div className="login-card">
        <h1>Create Admin</h1>
        <p className="subtitle">Add a new administrator</p>

        <label>First Name</label>
        <input
          type="text"
          value={firstName}
          onChange={(e) => setFirstName(e.target.value)}
        />

        <label>Last Name</label>
        <input
          type="text"
          value={lastName}
          onChange={(e) => setLastName(e.target.value)}
        />

        <label>Email</label>
        <input
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
        />

        <label>Password</label>
        <input
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />

        <label>Confirm Password</label>
        <input
          type="password"
          value={confirmPassword}
          onChange={(e) => setConfirmPassword(e.target.value)}
        />

        <button onClick={handleCreateAdmin}>Create Admin</button>
      </div>
    </div>
  );
}
