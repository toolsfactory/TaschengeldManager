import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { AuthProvider } from './contexts/AuthContext';
import { ThemeProvider } from './contexts/ThemeContext';
import { ProtectedRoute } from './components/ProtectedRoute';
import { Layout } from './components/Layout';
import {
  Login,
  Register,
  Dashboard,
  Accounts,
  AccountDetails,
  TransactionHistory,
  FamilyManage,
  FamilyCreate,
  AddChild,
  InviteMember,
  RecurringPayments,
  MoneyRequests,
  MyRequests,
  MfaSetup,
  Cashbook,
} from './pages';
import { UserRole } from './types';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 1,
      refetchOnWindowFocus: false,
    },
  },
});

function App() {
  return (
    <ThemeProvider>
      <QueryClientProvider client={queryClient}>
        <BrowserRouter>
          <AuthProvider>
            <Routes>
            {/* Public routes */}
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />

            {/* Protected routes with Layout */}
            <Route
              path="/dashboard"
              element={
                <ProtectedRoute>
                  <Layout>
                    <Dashboard />
                  </Layout>
                </ProtectedRoute>
              }
            />

            {/* Account routes - Parent only */}
            <Route
              path="/accounts"
              element={
                <ProtectedRoute allowedRoles={[UserRole.Parent]}>
                  <Layout>
                    <Accounts />
                  </Layout>
                </ProtectedRoute>
              }
            />
            <Route
              path="/accounts/:id"
              element={
                <ProtectedRoute allowedRoles={[UserRole.Parent, UserRole.Relative]}>
                  <Layout>
                    <AccountDetails />
                  </Layout>
                </ProtectedRoute>
              }
            />
            <Route
              path="/accounts/:id/history"
              element={
                <ProtectedRoute allowedRoles={[UserRole.Parent, UserRole.Relative]}>
                  <Layout>
                    <TransactionHistory />
                  </Layout>
                </ProtectedRoute>
              }
            />

            {/* Child's own transaction history */}
            <Route
              path="/account/history"
              element={
                <ProtectedRoute allowedRoles={[UserRole.Child]}>
                  <Layout>
                    <TransactionHistory />
                  </Layout>
                </ProtectedRoute>
              }
            />

            {/* Cashbook - Child */}
            <Route
              path="/cashbook"
              element={
                <ProtectedRoute allowedRoles={[UserRole.Child]}>
                  <Layout>
                    <Cashbook />
                  </Layout>
                </ProtectedRoute>
              }
            />

            {/* Cashbooks - Parent overview */}
            <Route
              path="/cashbooks"
              element={
                <ProtectedRoute allowedRoles={[UserRole.Parent]}>
                  <Layout>
                    <Cashbook />
                  </Layout>
                </ProtectedRoute>
              }
            />

            {/* Cashbook for specific account - Parent */}
            <Route
              path="/accounts/:accountId/cashbook"
              element={
                <ProtectedRoute allowedRoles={[UserRole.Parent]}>
                  <Layout>
                    <Cashbook />
                  </Layout>
                </ProtectedRoute>
              }
            />

            {/* Family routes - Parent only */}
            <Route
              path="/family"
              element={
                <ProtectedRoute allowedRoles={[UserRole.Parent]}>
                  <Layout>
                    <FamilyManage />
                  </Layout>
                </ProtectedRoute>
              }
            />
            <Route
              path="/family/create"
              element={
                <ProtectedRoute allowedRoles={[UserRole.Parent]}>
                  <Layout>
                    <FamilyCreate />
                  </Layout>
                </ProtectedRoute>
              }
            />
            <Route
              path="/family/children/add"
              element={
                <ProtectedRoute allowedRoles={[UserRole.Parent]}>
                  <Layout>
                    <AddChild />
                  </Layout>
                </ProtectedRoute>
              }
            />
            <Route
              path="/family/invite"
              element={
                <ProtectedRoute allowedRoles={[UserRole.Parent]}>
                  <Layout>
                    <InviteMember />
                  </Layout>
                </ProtectedRoute>
              }
            />

            {/* Recurring Payments - Parent only */}
            <Route
              path="/recurring-payments"
              element={
                <ProtectedRoute allowedRoles={[UserRole.Parent]}>
                  <Layout>
                    <RecurringPayments />
                  </Layout>
                </ProtectedRoute>
              }
            />

            {/* Money Requests - Parent */}
            <Route
              path="/money-requests"
              element={
                <ProtectedRoute allowedRoles={[UserRole.Parent]}>
                  <Layout>
                    <MoneyRequests />
                  </Layout>
                </ProtectedRoute>
              }
            />

            {/* My Requests - Child only */}
            <Route
              path="/my-requests"
              element={
                <ProtectedRoute allowedRoles={[UserRole.Child]}>
                  <Layout>
                    <MyRequests />
                  </Layout>
                </ProtectedRoute>
              }
            />

            {/* MFA Setup - All authenticated users */}
            <Route
              path="/settings/mfa"
              element={
                <ProtectedRoute>
                  <Layout>
                    <MfaSetup />
                  </Layout>
                </ProtectedRoute>
              }
            />

            {/* Redirects */}
            <Route path="/" element={<Navigate to="/dashboard" replace />} />
            <Route path="*" element={<Navigate to="/dashboard" replace />} />
            </Routes>
          </AuthProvider>
        </BrowserRouter>
      </QueryClientProvider>
    </ThemeProvider>
  );
}

export default App;
