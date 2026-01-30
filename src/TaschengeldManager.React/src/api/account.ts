import { apiClient } from './client';
import type {
  AccountDto,
  TransactionDto,
  DepositRequest,
  WithdrawRequest,
  GiftRequest,
  SetInterestRequest,
} from '../types';

export const accountApi = {
  async getAccount(id: string): Promise<AccountDto> {
    return apiClient.get<AccountDto>(`/account/${id}`);
  },

  async getMyAccount(): Promise<AccountDto> {
    return apiClient.get<AccountDto>('/account/me');
  },

  async getFamilyAccounts(familyId: string): Promise<AccountDto[]> {
    return apiClient.get<AccountDto[]>(`/account/family/${familyId}`);
  },

  async deposit(accountId: string, data: DepositRequest): Promise<TransactionDto> {
    return apiClient.post<TransactionDto>(`/account/${accountId}/deposit`, data);
  },

  async withdraw(data: WithdrawRequest): Promise<TransactionDto> {
    return apiClient.post<TransactionDto>('/account/withdraw', data);
  },

  async gift(accountId: string, data: GiftRequest): Promise<TransactionDto> {
    return apiClient.post<TransactionDto>(`/account/${accountId}/gift`, data);
  },

  async getTransactions(
    accountId: string,
    page: number = 1,
    pageSize: number = 20
  ): Promise<TransactionDto[]> {
    return apiClient.get<TransactionDto[]>(
      `/account/${accountId}/transactions?page=${page}&pageSize=${pageSize}`
    );
  },

  async getMyTransactions(page: number = 1, pageSize: number = 20): Promise<TransactionDto[]> {
    return apiClient.get<TransactionDto[]>(
      `/account/transactions?page=${page}&pageSize=${pageSize}`
    );
  },

  async setInterest(accountId: string, data: SetInterestRequest): Promise<void> {
    await apiClient.post(`/account/${accountId}/interest`, data);
  },

  async calculateInterest(accountId: string): Promise<TransactionDto | null> {
    return apiClient.post<TransactionDto | null>(`/account/${accountId}/interest/calculate`);
  },
};
