import { useState } from "react";
import api from "../api/httpClient";
import { useAuth } from "../auth/AuthContext";
import { useNavigate, Link } from "react-router-dom";
import "./Login.css";

function Login() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const { login } = useAuth();
  const navigate = useNavigate();

const handleLogin = async () => {
  try {
    const response = await api.post("/auth/login", {
      email,
      password,
    });

    login(response.data.token);
    navigate("/dashboard");
  } catch (error) {
    if (!error.response) {
      alert("Service unavailable. Please try again later.");
      return;
    }

    if (error.response.status === 401) {
      alert("Invalid email or password.");
      return;
    }

    if (error.response.status === 503) {
      alert("Authentication service is unavailable.");
      return;
    }

    alert("Unexpected error occurred.");
  }
};


  return (
    <div className="login-page">
      <div className="login-card">
        <h1>Welcome Back</h1>
        <p className="subtitle">Login to your account</p>

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

        <button onClick={handleLogin}>Login</button>

        <p className="signup-text">
          Don’t have an account? <Link to="/register">Sign Up</Link>
        </p>
      </div>
    </div>
  );
}

export default Login;
