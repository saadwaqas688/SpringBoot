import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { courseService, Course } from '../services/api';
import './Courses.css';

const MyCourses: React.FC = () => {
  const [courses, setCourses] = useState<Course[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadMyCourses();
  }, []);

  const loadMyCourses = async () => {
    try {
      const data = await courseService.getMyCourses();
      setCourses(data);
    } catch (error) {
      console.error('Failed to load courses:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <div className="loading">Loading your courses...</div>;
  }

  return (
    <div className="courses">
      <h1>My Courses</h1>
      {courses.length === 0 ? (
        <div className="no-courses">
          <p>You haven't enrolled in any courses yet.</p>
          <Link to="/courses" className="btn btn-primary">
            Browse Courses
          </Link>
        </div>
      ) : (
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
                  Continue Learning
                </Link>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default MyCourses;

