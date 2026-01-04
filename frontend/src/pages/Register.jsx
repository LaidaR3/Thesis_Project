import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import api from "../api/httpClient";
import "./Login.css"; 

export default function Register() {
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const navigate = useNavigate();

  const handleRegister = async () => {
    if (password !== confirmPassword) {
        alert("Passwords do not match");
        return;
    }

    try {
        await api.post("/auth/register", {
            firstName,
            lastName,
            email,
            password,
        });

        alert("Account created successfully");
        navigate("/");
    } catch {
        alert("Registration failed");
    }
  };

    return (
    <div className="login-page">
      <div className="login-card">
            <h1>Create Account</h1>
            <p className="subtitle">Sign up to get started</p>
            <label>First Name</label>
            <input
                type="firstname"
                value={firstName}
                onChange={(e) => setFirstName(e.target.value)}
            />
            <label>Last Name</label>
            <input
                type="lastname"
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

            <button onClick={handleRegister}>Sign Up</button>

            <p className="signup-text">
              Already have an account? <Link to="/">Login</Link>
            </p>
      </div>
    </div>
    );
}
