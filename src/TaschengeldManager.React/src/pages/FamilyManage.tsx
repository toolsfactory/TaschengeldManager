import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { familyApi } from '../api';
import { UserRole, type FamilyDto, type FamilyMemberDto, type ChildDto } from '../types';

export function FamilyManage() {
  const [families, setFamilies] = useState<FamilyDto[]>([]);
  const [selectedFamily, setSelectedFamily] = useState<FamilyDto | null>(null);
  const [members, setMembers] = useState<FamilyMemberDto[]>([]);
  const [children, setChildren] = useState<ChildDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const loadFamilies = async () => {
      try {
        const data = await familyApi.getMyFamilies();
        setFamilies(data);
        if (data.length > 0) {
          setSelectedFamily(data[0]);
        }
      } catch (error) {
        console.error('Failed to load families:', error);
      } finally {
        setIsLoading(false);
      }
    };

    loadFamilies();
  }, []);

  useEffect(() => {
    const loadFamilyData = async () => {
      if (!selectedFamily) return;

      try {
        const [membersData, childrenData] = await Promise.all([
          familyApi.getMembers(selectedFamily.id),
          familyApi.getChildren(selectedFamily.id),
        ]);
        setMembers(membersData);
        setChildren(childrenData);
      } catch (error) {
        console.error('Failed to load family data:', error);
      }
    };

    loadFamilyData();
  }, [selectedFamily]);

  const handleRemoveChild = async (childId: string) => {
    if (!selectedFamily) return;
    if (!confirm('Möchtest du dieses Kind wirklich entfernen?')) return;

    try {
      await familyApi.removeChild(selectedFamily.id, childId);
      setChildren((prev) => prev.filter((c) => c.id !== childId));
    } catch (error) {
      console.error('Failed to remove child:', error);
    }
  };

  const handleRemoveRelative = async (relativeId: string) => {
    if (!selectedFamily) return;
    if (!confirm('Möchtest du dieses Familienmitglied wirklich entfernen?')) return;

    try {
      await familyApi.removeRelative(selectedFamily.id, relativeId);
      setMembers((prev) => prev.filter((m) => m.id !== relativeId));
    } catch (error) {
      console.error('Failed to remove relative:', error);
    }
  };

  const getRoleLabel = (role: UserRole) => {
    switch (role) {
      case UserRole.Parent:
        return 'Elternteil';
      case UserRole.Child:
        return 'Kind';
      case UserRole.Relative:
        return 'Verwandte/r';
      default:
        return role;
    }
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-12">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 dark:border-blue-400"></div>
      </div>
    );
  }

  if (families.length === 0) {
    return (
      <div className="card text-center py-12">
        <span className="text-6xl">👨‍👩‍👧‍👦</span>
        <h2 className="text-xl font-semibold text-gray-900 dark:text-gray-100 mt-4">Keine Familie gefunden</h2>
        <p className="text-gray-600 dark:text-gray-400 mt-2">Erstelle eine Familie, um loszulegen.</p>
        <Link to="/family/create" className="btn-primary mt-4 inline-block">
          Familie erstellen
        </Link>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100">Familie verwalten</h1>
        {families.length > 1 && (
          <select
            value={selectedFamily?.id}
            onChange={(e) => setSelectedFamily(families.find((f) => f.id === e.target.value) || null)}
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

      {selectedFamily && (
        <>
          {/* Family Info Card */}
          <div className="card">
            <div className="flex items-center justify-between">
              <div>
                <h2 className="text-xl font-semibold text-gray-900 dark:text-gray-100">{selectedFamily.name}</h2>
                <p className="text-sm text-gray-500 dark:text-gray-400 mt-1">
                  Erstellt am {new Date(selectedFamily.createdAt).toLocaleDateString('de-DE')}
                </p>
              </div>
              <div className="text-right">
                <p className="text-sm text-gray-500 dark:text-gray-400">Familiencode</p>
                <p className="text-2xl font-mono font-bold text-blue-600 dark:text-blue-400 tracking-wider">
                  {selectedFamily.familyCode}
                </p>
              </div>
            </div>
          </div>

          {/* Quick Actions */}
          <div className="grid grid-cols-2 gap-4">
            <Link to="/family/children/add" className="card hover:shadow-lg dark:hover:shadow-gray-900/50 transition-shadow">
              <div className="flex items-center space-x-3">
                <span className="text-3xl">➕</span>
                <div>
                  <p className="font-medium text-gray-900 dark:text-gray-100">Kind hinzufügen</p>
                  <p className="text-sm text-gray-500 dark:text-gray-400">Neues Kinderkonto erstellen</p>
                </div>
              </div>
            </Link>
            <Link to="/family/invite" className="card hover:shadow-lg dark:hover:shadow-gray-900/50 transition-shadow">
              <div className="flex items-center space-x-3">
                <span className="text-3xl">📧</span>
                <div>
                  <p className="font-medium text-gray-900 dark:text-gray-100">Einladen</p>
                  <p className="text-sm text-gray-500 dark:text-gray-400">Verwandte einladen</p>
                </div>
              </div>
            </Link>
          </div>

          {/* Children */}
          <div className="card">
            <h3 className="text-lg font-semibold text-gray-900 dark:text-gray-100 mb-4">Kinder</h3>
            {children.length === 0 ? (
              <p className="text-gray-500 dark:text-gray-400 text-center py-4">Noch keine Kinder hinzugefügt.</p>
            ) : (
              <div className="space-y-3">
                {children.map((child) => (
                  <div
                    key={child.id}
                    className="flex items-center justify-between p-4 bg-gray-50 dark:bg-gray-700 rounded-lg"
                  >
                    <div className="flex items-center space-x-3">
                      <div className="w-12 h-12 bg-blue-100 dark:bg-blue-900 rounded-full flex items-center justify-center">
                        <span className="text-blue-600 dark:text-blue-400 font-bold text-lg">
                          {child.nickname.charAt(0).toUpperCase()}
                        </span>
                      </div>
                      <div>
                        <p className="font-medium text-gray-900 dark:text-gray-100">{child.nickname}</p>
                        <p className="text-sm text-gray-500 dark:text-gray-400">Guthaben: {child.balance.toFixed(2)} €</p>
                      </div>
                    </div>
                    <div className="flex items-center space-x-2">
                      {child.accountId && (
                        <Link
                          to={`/accounts/${child.accountId}`}
                          className="btn-secondary text-sm"
                        >
                          Konto →
                        </Link>
                      )}
                      <button
                        onClick={() => handleRemoveChild(child.id)}
                        className="text-red-600 dark:text-red-400 hover:text-red-700 dark:hover:text-red-300 p-2"
                        title="Kind entfernen"
                      >
                        🗑️
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>

          {/* Members */}
          <div className="card">
            <h3 className="text-lg font-semibold text-gray-900 dark:text-gray-100 mb-4">Familienmitglieder</h3>
            <div className="space-y-3">
              {members.map((member) => (
                <div
                  key={member.id}
                  className="flex items-center justify-between p-4 bg-gray-50 dark:bg-gray-700 rounded-lg"
                >
                  <div className="flex items-center space-x-3">
                    <div
                      className={`w-12 h-12 rounded-full flex items-center justify-center ${
                        member.role === UserRole.Parent
                          ? 'bg-green-100 dark:bg-green-900'
                          : member.role === UserRole.Relative
                          ? 'bg-purple-100 dark:bg-purple-900'
                          : 'bg-blue-100 dark:bg-blue-900'
                      }`}
                    >
                      <span
                        className={`font-bold text-lg ${
                          member.role === UserRole.Parent
                            ? 'text-green-600 dark:text-green-400'
                            : member.role === UserRole.Relative
                            ? 'text-purple-600 dark:text-purple-400'
                            : 'text-blue-600 dark:text-blue-400'
                        }`}
                      >
                        {member.nickname.charAt(0).toUpperCase()}
                      </span>
                    </div>
                    <div>
                      <p className="font-medium text-gray-900 dark:text-gray-100">{member.nickname}</p>
                      <p className="text-sm text-gray-500 dark:text-gray-400">{getRoleLabel(member.role)}</p>
                      {member.email && (
                        <p className="text-xs text-gray-400 dark:text-gray-500">{member.email}</p>
                      )}
                    </div>
                  </div>
                  {member.role === UserRole.Relative && (
                    <button
                      onClick={() => handleRemoveRelative(member.id)}
                      className="text-red-600 dark:text-red-400 hover:text-red-700 dark:hover:text-red-300 p-2"
                      title="Entfernen"
                    >
                      🗑️
                    </button>
                  )}
                </div>
              ))}
            </div>
          </div>
        </>
      )}
    </div>
  );
}
