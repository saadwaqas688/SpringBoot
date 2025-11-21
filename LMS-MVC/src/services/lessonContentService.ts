import api from './api';
import { LessonContent } from './api';

export const lessonContentService = {
  getByLessonId: async (lessonId: string): Promise<LessonContent[]> => {
    const response = await api.get<LessonContent[]>(`/lessoncontent/lesson/${lessonId}`);
    return response.data;
  },

  getById: async (id: string): Promise<LessonContent> => {
    const response = await api.get<LessonContent>(`/lessoncontent/${id}`);
    return response.data;
  },

  create: async (content: Partial<LessonContent>): Promise<LessonContent> => {
    const response = await api.post<LessonContent>('/lessoncontent', content);
    return response.data;
  },

  update: async (id: string, content: Partial<LessonContent>): Promise<LessonContent> => {
    const response = await api.put<LessonContent>(`/lessoncontent/${id}`, content);
    return response.data;
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/lessoncontent/${id}`);
  },
};

