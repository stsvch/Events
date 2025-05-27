import api from './axiosInstance';

export function getCategories() {
  return api.get('/categories');
}

export function getCategoryById(categoryId) {
  return api.get(`/categories/${categoryId}`);
}

export function createCategory(categoryData) {
  return api.post('/categories', categoryData);
}

export function deleteCategory(categoryId) {
  return api.delete(`/categories/${categoryId}`);
}
