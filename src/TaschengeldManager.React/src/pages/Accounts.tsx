import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { accountApi, familyApi } from '../api';
import type { AccountDto, FamilyDto } from '../types';

export function Accounts() {
  const [accounts, setAccounts] = useState<AccountDto[]>([]);
  const [families, setFamilies] = useState<FamilyDto[]>([]);
  const [selectedFamilyId, setSelectedFamilyId] = useState<string>('');
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const loadFamilies = async () => {
      try {
        const data = await familyApi.getMyFamilies();
        setFamilies(data);
        if (data.length > 0) {
          setSelectedFamilyId(data[0].id);
        }
      } catch (error) {
        console.error('Failed to load families:', error);
      }
    };

    loadFamilies();
  }, []);

  useEffect(() => {
    const loadAccounts = async () => {
      if (!selectedFamilyId) {
        setIsLoading(false);
        return;
      }

      setIsLoading(true);
      try {
        const data = await accountApi.getFamilyAccounts(selectedFamilyId);
        setAccounts(data);
      } catch (error) {
        console.error('Failed to load accounts:', error);
      } finally {
        setIsLoading(false);
      }
    };

    loadAccounts();
  }, [selectedFamilyId]);

  const totalBalance = accounts.reduce((sum, acc) => sum + acc.balance, 0);

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100">Kinderkonten</h1>
        {families.length > 1 && (
          <select
            value={selectedFamilyId}
            onChange={(e) => setSelectedFamilyId(e.target.value)}
            className="input w-auto"
          >
            {families.map((family) => (
              <option key={family.id} value={family.id}>
                {family.name}
              </option>
            ))}
          </select>
        )}
      </div>

      {/* Summary Card */}
      <div className="card bg-gradient-to-r from-green-500 to-green-600 dark:from-green-600 dark:to-green-700 text-white">
        <div className="flex items-center justify-between">
          <div>
            <p className="text-green-100">Gesamtguthaben aller Kinder</p>
            <p className="text-3xl font-bold mt-1">{totalBalance.toFixed(2)} €</p>
          </div>
          <div className="text-5xl opacity-50">💰</div>
        </div>
      </div>

      {isLoading ? (
        <div className="flex items-center justify-center py-12">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 dark:border-blue-400"></div>
        </div>
      ) : accounts.length === 0 ? (
        <div className="card text-center py-12">
          <span className="text-6xl">👶</span>
          <h2 className="text-xl font-semibold text-gray-900 dark:text-gray-100 mt-4">Keine Kinderkonten</h2>
          <p className="text-gray-600 dark:text-gray-400 mt-2">Füge ein Kind hinzu, um ein Konto zu erstellen.</p>
          <Link to="/family/children/add" className="btn-primary mt-4 inline-block">
            Kind hinzufügen
          </Link>
        </div>
      ) : (
        <div className="grid gap-4">
          {accounts.map((account) => (
            <Link
              key={account.id}
              to={`/accounts/${account.id}`}
              className="card hover:shadow-lg dark:hover:shadow-gray-900/50 transition-shadow"
            >
              <div className="flex items-center justify-between">
                <div className="flex items-center space-x-4">
                  <div className="w-14 h-14 bg-blue-100 dark:bg-blue-900 rounded-full flex items-center justify-center">
                    <span className="text-blue-600 dark:text-blue-400 font-bold text-xl">
                      {account.ownerName.charAt(0).toUpperCase()}
                    </span>
                  </div>
                  <div>
                    <h3 className="font-semibold text-gray-900 dark:text-gray-100 text-lg">{account.ownerName}</h3>
                    <p className="text-sm text-gray-500 dark:text-gray-400">
                      Erstellt am {new Date(account.createdAt).toLocaleDateString('de-DE')}
                    </p>
                  </div>
                </div>
                <div className="text-right">
                  <p className="text-2xl font-bold text-gray-900 dark:text-gray-100">{account.balance.toFixed(2)} €</p>
                  {account.interestEnabled && (
                    <p className="text-sm text-green-600 dark:text-green-400">
                      📈 {account.interestRate}% Zinsen
                    </p>
                  )}
                </div>
              </div>
            </Link>
          ))}
        </div>
      )}
    </div>
  );
}
