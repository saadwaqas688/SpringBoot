import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import './Home.css';

const Home: React.FC = () => {
  const { isAuthenticated } = useAuth();

  return (
    <div className="home">
      <div className="hero">
        <h1>Welcome to Learning Management System</h1>
        <p>Your gateway to knowledge and skill development</p>
        {isAuthenticated ? (
          <Link to="/courses" className="btn btn-primary">
            Browse Courses
          </Link>
        ) : (
          <div>
            <Link to="/login" className="btn btn-primary">
              Login
            </Link>
            <Link to="/register" className="btn btn-secondary" style={{ marginLeft: '10px' }}>
              Register
            </Link>
          </div>
        )}
      </div>
    </div>
  );
};

export default Home;

