import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import './Navbar.css';

const Navbar: React.FC = () => {
  const { isAuthenticated, user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/');
  };

  return (
    <nav className="navbar">
      <div className="navbar-container">
        <Link to="/" className="navbar-brand">
          LMS
        </Link>
        <div className="navbar-menu">
          {isAuthenticated ? (
            <>
              <Link to="/courses" className="navbar-link">
                All Courses
              </Link>
              <Link to="/my-courses" className="navbar-link">
                My Courses
              </Link>
              {user?.role === 'ADMIN' && (
                <Link to="/courses" className="navbar-link">
                  Admin
                </Link>
              )}
              <span className="navbar-user">Welcome, {user?.name}</span>
              <button onClick={handleLogout} className="btn btn-secondary">
                Logout
              </button>
            </>
          ) : (
            <>
              <Link to="/login" className="navbar-link">
                Login
              </Link>
              <Link to="/register" className="navbar-link">
                Register
              </Link>
            </>
          )}
        </div>
      </div>
    </nav>
  );
};

export default Navbar;

