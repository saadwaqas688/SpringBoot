import { useState } from "react";

export const useLessonSelection = () => {
  const [selectedLessonForSlides, setSelectedLessonForSlides] = useState<any>(null);
  const [lastAutoSelectedLessonId, setLastAutoSelectedLessonId] = useState<string | null>(null);

  const handleLessonClick = (lesson: any) => {
    const lessonId = lesson.id || lesson.Id;
    const lessonType = lesson.lessonType || lesson.LessonType || "standard";
    
    // If it's a quiz type lesson, select it
    if (lessonType.toLowerCase() === "quiz") {
      setSelectedLessonForSlides({ ...lesson, id: lessonId, Id: lessonId });
      setLastAutoSelectedLessonId(null);
      return;
    }
    
    // Toggle selection - if same lesson is clicked, deselect it
    if (
      selectedLessonForSlides &&
      (selectedLessonForSlides.id || selectedLessonForSlides.Id) === lessonId
    ) {
      setSelectedLessonForSlides(null);
      setLastAutoSelectedLessonId(null);
    } else {
      setSelectedLessonForSlides({ ...lesson, id: lessonId, Id: lessonId });
      setLastAutoSelectedLessonId(null);
    }
  };

  const isQuizLesson = selectedLessonForSlides 
    ? (selectedLessonForSlides.lessonType || selectedLessonForSlides.LessonType || "standard").toLowerCase() === "quiz"
    : false;

  const selectedLessonId = selectedLessonForSlides?.id || selectedLessonForSlides?.Id || null;

  return {
    selectedLessonForSlides,
    setSelectedLessonForSlides,
    selectedLessonId,
    isQuizLesson,
    handleLessonClick,
    lastAutoSelectedLessonId,
    setLastAutoSelectedLessonId,
  };
};

