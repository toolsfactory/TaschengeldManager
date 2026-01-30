import { apiClient } from './client';
import type {
  RecurringPaymentDto,
  CreateRecurringPaymentRequest,
  UpdateRecurringPaymentRequest,
} from '../types';

export const recurringPaymentApi = {
  async getAll(): Promise<RecurringPaymentDto[]> {
    return apiClient.get<RecurringPaymentDto[]>('/recurring-payments');
  },

  async getById(id: string): Promise<RecurringPaymentDto> {
    return apiClient.get<RecurringPaymentDto>(`/recurring-payments/${id}`);
  },

  async create(data: CreateRecurringPaymentRequest): Promise<RecurringPaymentDto> {
    return apiClient.post<RecurringPaymentDto>('/recurring-payments', data);
  },

  async update(id: string, data: UpdateRecurringPaymentRequest): Promise<RecurringPaymentDto> {
    return apiClient.put<RecurringPaymentDto>(`/recurring-payments/${id}`, data);
  },

  async delete(id: string): Promise<void> {
    await apiClient.delete(`/recurring-payments/${id}`);
  },

  async toggleActive(id: string): Promise<RecurringPaymentDto> {
    return apiClient.post<RecurringPaymentDto>(`/recurring-payments/${id}/toggle`);
  },
};
