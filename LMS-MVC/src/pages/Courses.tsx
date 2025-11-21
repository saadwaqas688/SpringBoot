import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { courseService, Course } from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import './Courses.css';

const Courses: React.FC = () => {
  const [courses, setCourses] = useState<Course[]>([]);
  const [loading, setLoading] = useState(true);
  const { user } = useAuth();

  useEffect(() => {
    loadCourses();
  }, []);

  const loadCourses = async () => {
    try {
      const data = await courseService.getAll();
      setCourses(data);
    } catch (error) {
      console.error('Failed to load courses:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <div className="loading">Loading courses...</div>;
  }

  return (
    <div className="courses">
      <div className="courses-header">
        <h1>All Courses</h1>
        {user?.role === 'ADMIN' && (
          <Link to="/courses/create" className="btn btn-primary">
            Create Course
          </Link>
        )}
      </div>
      <div className="courses-grid">
        {courses.map((course) => (
          <div key={course.id} className="course-card">
            {course.thumbnailUrl && (
              <img src={course.thumbnailUrl} alt={course.title} className="course-thumbnail" />
            )}
            <div className="course-content">
              <h3>{course.title}</h3>
              <p className="course-description">{course.description}</p>
              <div className="course-meta">
                <span className="course-category">{course.category}</span>
                <span className="course-level">{course.level}</span>
              </div>
              <div className="course-stats">
                <span>{course.lessonCount} Lessons</span>
                <span>{course.enrollmentCount} Enrollments</span>
              </div>
              <Link to={`/courses/${course.id}`} className="btn btn-primary">
                View Course
              </Link>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default Courses;

