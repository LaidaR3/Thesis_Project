import { useAuth } from "../auth/AuthContext";
import { NavLink, useNavigate } from "react-router-dom";
import "./Sidebar.css";

import { IoAdd, IoLogOutOutline } from "react-icons/io5";
import { FaUsers } from "react-icons/fa6";
import { FaUserAlt } from "react-icons/fa";
import { TbBrandGoogleAnalytics } from "react-icons/tb";
import { LuLogs } from "react-icons/lu";



export default function Sidebar() {
    const { logout, user } = useAuth();
    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        navigate("/");
    };

    return (
        <aside className="sidebar">
            <div className="sidebar-header">
                <span className="logo"></span>
                <h2>Admin Panel</h2>
            </div>

            <nav className="sidebar-menu">
                <NavLink to="/" className="menu-item">
                    <span className="icon"><TbBrandGoogleAnalytics /></span>
                    <span>Dashboard</span>
                </NavLink>

                <NavLink to="/dashboard" className="menu-item">
                    <span className="icon"><LuLogs /></span>
                    <span>Audit Logs</span>
                </NavLink>

                <NavLink to="/profile" className="menu-item">
                    <span className="icon"><FaUserAlt /></span>
                    <span>My Profile</span>
                </NavLink>

                <NavLink to="/users" className="menu-item">
                    <span className="icon"><FaUsers /></span>
                    <span>All Users</span>
                </NavLink>

                {user?.roles?.includes("Admin") && (
                    <NavLink to="/create-admin" className="menu-item highlight">
                        <span className="icon"><IoAdd /></span>
                        <span>Create Admin</span>
                    </NavLink>
                )}
            </nav>

            <div className="sidebar-footer">
                <button className="logout-btn" onClick={handleLogout}>
                    <IoLogOutOutline />
                    Logout
                </button>
            </div>
        </aside>
    );
}
