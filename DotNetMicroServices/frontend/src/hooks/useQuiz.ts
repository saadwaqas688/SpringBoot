import { useState, useEffect, useCallback, useRef } from "react";
import {
  useGetQuizzesByLessonQuery,
  useGetQuestionsByQuizQuery,
  useCreateQuizAttemptMutation,
  useAddQuizAnswerMutation,
} from "@/services/quizzes-api";

export const useQuiz = (lessonId: string | null, isQuizLesson: boolean) => {
  const [currentQuestionIndex, setCurrentQuestionIndex] = useState(0);
  const [selectedAnswers, setSelectedAnswers] = useState<{ [questionId: string]: string }>({});
  const [attemptId, setAttemptId] = useState<string | null>(null);
  
  // Get user from localStorage
  const getUserFromStorage = () => {
    if (typeof window === "undefined") return null;
    try {
      const userInfo = localStorage.getItem("user_info");
      if (userInfo) {
        return JSON.parse(userInfo);
      }
    } catch (error) {
      console.error("Error reading user info from localStorage:", error);
    }
    return null;
  };
  
  const user = getUserFromStorage();
  const userId = user?.id || user?.Id || user?._id || "";
  const userRole = user?.role || user?.Role || "";
  const isUserRole = userRole.toLowerCase() === "user";
  
  const [createQuizAttempt, { isLoading: isCreatingAttempt }] = useCreateQuizAttemptMutation();
  const [addQuizAnswer, { isLoading: isSavingAnswer }] = useAddQuizAnswerMutation();

  const { data: quizData, isLoading: quizLoading, error: quizError } = useGetQuizzesByLessonQuery(
    lessonId || "",
    {
      skip: !lessonId || !isQuizLesson,
      refetchOnMountOrArgChange: true,
    }
  );

  // Handle different response structures - backend returns single Quiz object
  const quiz = quizData?.data || quizData;
  
  // Extract quizId - Quiz model uses Id (capital I)
  const quizId = quiz?.id || quiz?.Id || quiz?._id;
  
  // If quizId is an object (MongoDB ObjectId format), extract the string
  const quizIdString = typeof quizId === 'object' && quizId !== null && '$oid' in quizId
    ? (quizId as any).$oid
    : String(quizId || '');
  
  // Debug logging for quiz data
  useEffect(() => {
    if (isQuizLesson && lessonId) {
      console.log("=== QUIZ DEBUG ===");
      console.log("Lesson ID:", lessonId);
      console.log("Is Quiz Lesson:", isQuizLesson);
      console.log("Quiz Data:", quizData);
      console.log("Quiz Object:", quiz);
      console.log("Quiz ID (raw):", quizId);
      console.log("Quiz ID String:", quizIdString);
      console.log("Quiz Loading:", quizLoading);
      console.log("Quiz Error:", quizError);
    }
  }, [isQuizLesson, lessonId, quizData, quiz, quizId, quizIdString, quizLoading, quizError]);

  const { data: quizQuestionsData, isLoading: questionsLoading, error: questionsError, refetch: refetchQuestions } = useGetQuestionsByQuizQuery(
    quizIdString || "",
    {
      skip: !quizIdString || !isQuizLesson,
      refetchOnMountOrArgChange: true,
    }
  );

  // Handle different response structures for questions
  const quizQuestions = Array.isArray(quizQuestionsData?.data)
    ? quizQuestionsData.data
    : Array.isArray(quizQuestionsData?.data?.items)
    ? quizQuestionsData.data.items
    : Array.isArray(quizQuestionsData?.items)
    ? quizQuestionsData.items
    : Array.isArray(quizQuestionsData)
    ? quizQuestionsData
    : [];

  const sortedQuizQuestions = [...quizQuestions].sort((a: any, b: any) => 
    (a.order || a.Order || 0) - (b.order || b.Order || 0)
  );

  // Create quiz attempt when quiz loads (only for user role)
  // Use ref to prevent multiple attempts
  const attemptCreationRef = useRef<string | null>(null);
  
  useEffect(() => {
    // Reset attempt creation flag when lesson changes
    if (lessonId) {
      attemptCreationRef.current = null;
      setAttemptId(null); // Also reset attemptId
    }
  }, [lessonId]);
  
  useEffect(() => {
    // Only create attempt if all conditions are met and we haven't created one for this quiz
    const shouldCreateAttempt = 
      isQuizLesson && 
      quizIdString && 
      quizIdString.length > 0 &&
      userId && 
      userId.length > 0 &&
      isUserRole && 
      !attemptId && 
      !isCreatingAttempt &&
      attemptCreationRef.current !== quizIdString; // Haven't created for this quiz yet
    
    if (shouldCreateAttempt) {
      attemptCreationRef.current = quizIdString; // Mark as creating for this quiz
      const createAttempt = async () => {
        try {
          const result = await createQuizAttempt({
            userId: userId,
            quizId: quizIdString,
          }).unwrap();
          const createdAttemptId = result?.data?.id || result?.data?.Id || result?.id || result?.Id;
          if (createdAttemptId) {
            setAttemptId(createdAttemptId);
          } else {
            attemptCreationRef.current = null; // Reset if failed
          }
        } catch (error) {
          console.error("Error creating quiz attempt:", error);
          attemptCreationRef.current = null; // Reset on error so it can retry
        }
      };
      createAttempt();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isQuizLesson, quizIdString, userId, isUserRole, attemptId, isCreatingAttempt]);

  // Save answer when navigating to next question (only for user role)
  const saveCurrentAnswer = useCallback(async () => {
    if (!isUserRole || !attemptId || !quizIdString) return;
    
    const currentQuestion = sortedQuizQuestions[currentQuestionIndex];
    if (!currentQuestion) return;
    
    const questionId = currentQuestion.id || currentQuestion.Id || currentQuestion._id;
    const selectedOption = selectedAnswers[questionId];
    
    if (selectedOption !== undefined && selectedOption !== null && selectedOption !== "") {
      try {
        // Get the actual option value from the question
        const optionIndex = parseInt(selectedOption);
        const option = currentQuestion.options?.[optionIndex] || currentQuestion.Options?.[optionIndex];
        const optionValue = option?.value || option?.Value || selectedOption;
        
        await addQuizAnswer({
          id: attemptId,
          questionId: questionId,
          selectedOption: optionValue,
        }).unwrap();
        console.log("Answer saved successfully for question:", questionId);
      } catch (error) {
        console.error("Error saving answer:", error);
      }
    }
  }, [isUserRole, attemptId, currentQuestionIndex, sortedQuizQuestions, selectedAnswers, addQuizAnswer]);

  const handleQuestionNavigation = async (direction: "prev" | "next") => {
    // Save current answer before navigating (only for user role and when going to next)
    if (direction === "next" && isUserRole && attemptId) {
      await saveCurrentAnswer();
    }
    
    if (direction === "prev" && currentQuestionIndex > 0) {
      setCurrentQuestionIndex(currentQuestionIndex - 1);
    } else if (direction === "next" && currentQuestionIndex < sortedQuizQuestions.length - 1) {
      setCurrentQuestionIndex(currentQuestionIndex + 1);
    }
  };

  const handleAnswerSelect = (questionId: string, optionIndex: number) => {
    setSelectedAnswers({
      ...selectedAnswers,
      [questionId]: optionIndex.toString(),
    });
  };

  const resetQuiz = () => {
    setCurrentQuestionIndex(0);
    setSelectedAnswers({});
    setAttemptId(null); // Reset attempt ID when lesson changes
  };

  useEffect(() => {
    // Only reset when lessonId actually changes, not on every render
    if (lessonId) {
      resetQuiz();
    }
  }, [lessonId]);
  
  // Debug logging for questions
  useEffect(() => {
    if (isQuizLesson && quizIdString) {
      console.log("=== QUIZ QUESTIONS DEBUG ===");
      console.log("Quiz ID String:", quizIdString);
      console.log("Questions Data:", quizQuestionsData);
      console.log("Quiz Questions (raw):", quizQuestions);
      console.log("Quiz Questions (sorted):", sortedQuizQuestions);
      console.log("Questions Count:", sortedQuizQuestions.length);
      console.log("Current Question Index:", currentQuestionIndex);
      console.log("Current Question:", sortedQuizQuestions[currentQuestionIndex]);
      console.log("Questions Loading:", questionsLoading);
      console.log("Questions Error:", questionsError);
    }
  }, [quizIdString, quizQuestionsData, quizQuestions, sortedQuizQuestions, currentQuestionIndex, questionsLoading, questionsError, isQuizLesson]);

  return {
    quiz,
    quizId: quizIdString,
    quizLoading,
    quizError,
    quizQuestions: sortedQuizQuestions,
    questionsLoading,
    questionsError,
    currentQuestionIndex,
    selectedAnswers,
    handleQuestionNavigation,
    handleAnswerSelect,
    resetQuiz,
    refetchQuestions,
    attemptId,
    isUserRole,
  };
};

