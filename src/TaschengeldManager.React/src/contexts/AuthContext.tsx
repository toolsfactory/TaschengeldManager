import { createContext, useContext, useState, useEffect, useCallback, type ReactNode } from 'react';
import { authApi, apiClient } from '../api';
import type { UserDto, LoginRequest, ChildLoginRequest, RegisterRequest } from '../types';

interface AuthContextType {
  user: UserDto | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (data: LoginRequest) => Promise<{ mfaRequired: boolean; mfaToken?: string }>;
  childLogin: (data: ChildLoginRequest) => Promise<{ mfaRequired: boolean; mfaToken?: string }>;
  register: (data: RegisterRequest) => Promise<{ mfaSetupRequired: boolean }>;
  verifyTotp: (mfaToken: string, code: string) => Promise<void>;
  logout: () => Promise<void>;
  refreshUser: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<UserDto | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  const refreshUser = useCallback(async () => {
    try {
      const currentUser = await authApi.getCurrentUser();
      setUser(currentUser);
      localStorage.setItem('user', JSON.stringify(currentUser));
    } catch {
      setUser(null);
      apiClient.clearTokens();
    }
  }, []);

  useEffect(() => {
    const initAuth = async () => {
      if (apiClient.isAuthenticated()) {
        const storedUser = authApi.getStoredUser();
        if (storedUser) {
          setUser(storedUser);
        }
        try {
          await refreshUser();
        } catch {
          // Token might be invalid
          apiClient.clearTokens();
        }
      }
      setIsLoading(false);
    };

    initAuth();
  }, [refreshUser]);

  const login = async (data: LoginRequest) => {
    const response = await authApi.login(data);
    if ('mfaRequired' in response && response.mfaRequired) {
      return { mfaRequired: true, mfaToken: response.mfaToken };
    }
    if ('user' in response) {
      setUser(response.user);
    }
    return { mfaRequired: false };
  };

  const childLogin = async (data: ChildLoginRequest) => {
    const response = await authApi.childLogin(data);
    if ('mfaRequired' in response && response.mfaRequired) {
      return { mfaRequired: true, mfaToken: response.mfaToken };
    }
    if ('user' in response) {
      setUser(response.user);
    }
    return { mfaRequired: false };
  };

  const register = async (data: RegisterRequest) => {
    const response = await authApi.register(data);
    return { mfaSetupRequired: response.mfaSetupRequired };
  };

  const verifyTotp = async (mfaToken: string, code: string) => {
    const response = await authApi.verifyTotp({ mfaToken, code });
    setUser(response.user);
  };

  const logout = async () => {
    const refreshToken = localStorage.getItem('refreshToken');
    if (refreshToken) {
      try {
        await authApi.logout(refreshToken);
      } catch {
        // Ignore errors, just clear local state
      }
    }
    setUser(null);
    apiClient.clearTokens();
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        isAuthenticated: !!user,
        isLoading,
        login,
        childLogin,
        register,
        verifyTotp,
        logout,
        refreshUser,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}
