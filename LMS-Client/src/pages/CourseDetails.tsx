import React, { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { courseService, lessonService, enrollmentService, Course, Lesson } from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import './CourseDetails.css';

const CourseDetails: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const [course, setCourse] = useState<Course | null>(null);
  const [lessons, setLessons] = useState<Lesson[]>([]);
  const [loading, setLoading] = useState(true);
  const [enrolled, setEnrolled] = useState(false);
  const { user } = useAuth();

  useEffect(() => {
    if (id) {
      loadCourse();
      loadLessons();
      checkEnrollment();
    }
  }, [id]);

  const loadCourse = async () => {
    try {
      const data = await courseService.getById(id!);
      setCourse(data);
    } catch (error) {
      console.error('Failed to load course:', error);
    } finally {
      setLoading(false);
    }
  };

  const loadLessons = async () => {
    try {
      const data = await lessonService.getByCourseId(id!);
      setLessons(data);
    } catch (error) {
      console.error('Failed to load lessons:', error);
    }
  };

  const checkEnrollment = async () => {
    try {
      const enrollments = await enrollmentService.getMyEnrollments();
      setEnrolled(enrollments.some((e: any) => e.courseId === id));
    } catch (error) {
      console.error('Failed to check enrollment:', error);
    }
  };

  const handleEnroll = async () => {
    if (!user || !id) return;
    try {
      await enrollmentService.enroll(user.id, id);
      setEnrolled(true);
      alert('Successfully enrolled in course!');
    } catch (error: any) {
      alert(error.response?.data?.message || 'Failed to enroll');
    }
  };

  if (loading) {
    return <div className="loading">Loading course details...</div>;
  }

  if (!course) {
    return <div>Course not found</div>;
  }

  return (
    <div className="course-details">
      <div className="course-header">
        {course.thumbnailUrl && (
          <img src={course.thumbnailUrl} alt={course.title} className="course-header-image" />
        )}
        <div className="course-header-content">
          <h1>{course.title}</h1>
          <p className="course-description-full">{course.description}</p>
          <div className="course-info">
            <span>Category: {course.category}</span>
            <span>Level: {course.level}</span>
            <span>Lessons: {course.lessonCount}</span>
            <span>Enrollments: {course.enrollmentCount}</span>
          </div>
          {!enrolled && user?.role !== 'ADMIN' && (
            <button onClick={handleEnroll} className="btn btn-primary">
              Enroll in Course
            </button>
          )}
        </div>
      </div>

      <div className="lessons-section">
        <h2>Lessons</h2>
        {lessons.length === 0 ? (
          <p>No lessons available yet.</p>
        ) : (
          <div className="lessons-list">
            {lessons.map((lesson) => (
              <div key={lesson.id} className="lesson-item">
                <h3>{lesson.title}</h3>
                <p>{lesson.description}</p>
                {enrolled && (
                  <Link to={`/lessons/${lesson.id}`} className="btn btn-primary">
                    View Lesson
                  </Link>
                )}
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default CourseDetails;

