import { apiClient } from './client';
import type {
  MoneyRequestDto,
  CreateMoneyRequestRequest,
  RespondToMoneyRequestRequest,
} from '../types';

export const moneyRequestApi = {
  // For parents - get all requests in their family
  async getAll(): Promise<MoneyRequestDto[]> {
    return apiClient.get<MoneyRequestDto[]>('/money-requests/family');
  },

  // For children - get their own requests
  async getMyRequests(): Promise<MoneyRequestDto[]> {
    return apiClient.get<MoneyRequestDto[]>('/money-requests/my');
  },

  async getById(id: string): Promise<MoneyRequestDto> {
    return apiClient.get<MoneyRequestDto>(`/money-requests/${id}`);
  },

  // For children - create a new request
  async create(data: CreateMoneyRequestRequest): Promise<MoneyRequestDto> {
    return apiClient.post<MoneyRequestDto>('/money-requests', data);
  },

  // For parents - approve or reject
  async respond(id: string, data: RespondToMoneyRequestRequest): Promise<MoneyRequestDto> {
    return apiClient.post<MoneyRequestDto>(`/money-requests/${id}/respond`, data);
  },

  // For children - withdraw a pending request
  async withdraw(id: string): Promise<void> {
    await apiClient.post(`/money-requests/${id}/withdraw`);
  },
};
