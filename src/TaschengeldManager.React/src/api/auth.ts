import { apiClient } from './client';
import type {
  LoginRequest,
  LoginResponse,
  ChildLoginRequest,
  RegisterRequest,
  RegisterResponse,
  UserDto,
  VerifyTotpRequest,
  SetupTotpResponse,
  BackupCodesResponse,
  MfaRequiredResponse,
} from '../types';

export const authApi = {
  async register(data: RegisterRequest): Promise<RegisterResponse> {
    return apiClient.post<RegisterResponse>('/auth/register', data);
  },

  async login(data: LoginRequest): Promise<LoginResponse | MfaRequiredResponse> {
    const response = await apiClient.post<LoginResponse | MfaRequiredResponse>('/auth/login', data);
    if ('accessToken' in response) {
      apiClient.setTokens(response.accessToken, response.refreshToken);
      localStorage.setItem('user', JSON.stringify(response.user));
    }
    return response;
  },

  async childLogin(data: ChildLoginRequest): Promise<LoginResponse | MfaRequiredResponse> {
    const response = await apiClient.post<LoginResponse | MfaRequiredResponse>('/auth/login/child', data);
    if ('accessToken' in response) {
      apiClient.setTokens(response.accessToken, response.refreshToken);
      localStorage.setItem('user', JSON.stringify(response.user));
    }
    return response;
  },

  async verifyTotp(data: VerifyTotpRequest): Promise<LoginResponse> {
    const response = await apiClient.post<LoginResponse>('/auth/mfa/verify', data);
    apiClient.setTokens(response.accessToken, response.refreshToken);
    localStorage.setItem('user', JSON.stringify(response.user));
    return response;
  },

  async setupTotp(): Promise<SetupTotpResponse> {
    return apiClient.post<SetupTotpResponse>('/auth/mfa/totp/setup');
  },

  async activateTotp(setupToken: string, code: string): Promise<void> {
    await apiClient.post('/auth/mfa/totp/activate', { setupToken, code });
  },

  async generateBackupCodes(): Promise<BackupCodesResponse> {
    return apiClient.post<BackupCodesResponse>('/auth/mfa/backup-codes');
  },

  async logout(refreshToken: string): Promise<void> {
    try {
      await apiClient.post('/auth/logout', { refreshToken });
    } finally {
      apiClient.clearTokens();
    }
  },

  async logoutAll(): Promise<void> {
    try {
      await apiClient.post('/auth/logout/all');
    } finally {
      apiClient.clearTokens();
    }
  },

  async getCurrentUser(): Promise<UserDto> {
    return apiClient.get<UserDto>('/auth/me');
  },

  getStoredUser(): UserDto | null {
    const userJson = localStorage.getItem('user');
    if (userJson) {
      try {
        return JSON.parse(userJson) as UserDto;
      } catch {
        return null;
      }
    }
    return null;
  },
};
