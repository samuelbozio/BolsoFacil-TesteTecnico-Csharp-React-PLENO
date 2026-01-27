/**
 * Componente de Seção de Pessoas
 * 
 * Gerencia CRUD de pessoas com:
 * - Listagem de todas as pessoas com seus totais (receitas, despesas, saldo)
 * - Criação de novas pessoas
 * - Edição de dados existentes
 * - Deleção de pessoas (com cascade delete das transações)
 * - Exibição do resumo total geral
 */

import React, { useState } from 'react';
import { PersonDTO, PersonWithTotalsDTO, TransactionType } from '../types';
import { useAsync, useMutation, useForm, useNotification, useModal } from '../hooks/useCustomHooks';
import apiService from '../services/apiService';
import { NotificationContainer } from './Notification';

const PeopleSection: React.FC = () => {
  const { data: people, loading: peopleLoading, error, refetch } = useAsync(() => apiService.getPeople());
  const { data: summary, loading: summaryLoading } = useAsync(() => apiService.getPeopleTotalSummary());
  const { mutate: createPerson } = useMutation(apiService.createPerson);
  const { mutate: updatePerson } = useMutation((id: number, data: PersonDTO) => 
    apiService.updatePerson(id, data)
  );
  const { mutate: deletePerson } = useMutation(apiService.deletePerson);

  const { notifications, notify, removeNotification } = useNotification();
  const { isOpen: isFormOpen, open: openForm, close: closeForm } = useModal();
  const [editingPerson, setEditingPerson] = useState<PersonWithTotalsDTO | null>(null);

  const form = useForm({ name: '', age: 0 });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!form.values.name.trim()) {
      notify('Por favor, preencha o nome', 'warning');
      return;
    }

    if (form.values.age < 0 || form.values.age > 150) {
      notify('Idade deve estar entre 0 e 150', 'warning');
      return;
    }

    try {
      if (editingPerson) {
        await updatePerson(editingPerson.id, form.values);
        notify('Pessoa atualizada com sucesso!', 'success');
      } else {
        await createPerson(form.values);
        notify('Pessoa criada com sucesso!', 'success');
      }

      form.reset();
      setEditingPerson(null);
      closeForm();
      refetch();
    } catch (err: any) {
      notify(err.message || 'Erro ao salvar pessoa', 'error');
    }
  };

  const handleEdit = (person: PersonWithTotalsDTO) => {
    setEditingPerson(person);
    form.setValues({ name: person.name, age: person.age });
    openForm();
  };

  const handleDelete = async (id: number, name: string) => {
    if (!confirm(`Tem certeza que deseja deletar "${name}" e todas as suas transações?`)) {
      return;
    }

    try {
      await deletePerson(id);
      notify('Pessoa deletada com sucesso!', 'success');
      refetch();
    } catch (err: any) {
      notify(err.message || 'Erro ao deletar pessoa', 'error');
    }
  };

  const handleCloseForm = () => {
    closeForm();
    setEditingPerson(null);
    form.reset();
  };

  if (peopleLoading) {
    return <div className="loading"><div className="spinner"></div></div>;
  }

  return (
    <div>
      <NotificationContainer notifications={notifications} onRemove={removeNotification} />

      <div className="card">
        <div className="flex flex-between">
          <h2>Gerenciamento de Pessoas</h2>
          <button className="btn-primary" onClick={openForm}>+ Adicionar Pessoa</button>
        </div>
      </div>

      {/* Modal de Formulário */}
      {isFormOpen && (
        <div className="modal-overlay">
          <div className="modal">
            <div className="modal-header">
              <h3>{editingPerson ? 'Editar Pessoa' : 'Adicionar Pessoa'}</h3>
              <button className="close-btn" onClick={handleCloseForm}>×</button>
            </div>

            <form onSubmit={handleSubmit}>
              <div className="modal-body">
                <div className="form-group">
                  <label htmlFor="name">Nome *</label>
                  <input
                    id="name"
                    name="name"
                    type="text"
                    value={form.values.name}
                    onChange={form.handleChange}
                    placeholder="Ex: João Silva"
                    maxLength={200}
                    required
                  />
                </div>

                <div className="form-group">
                  <label htmlFor="age">Idade *</label>
                  <input
                    id="age"
                    name="age"
                    type="number"
                    value={form.values.age}
                    onChange={form.handleChange}
                    placeholder="Ex: 30"
                    min="0"
                    max="150"
                    required
                  />
                  {form.values.age > 0 && form.values.age < 18 && (
                    <p style={{ color: '#f59e0b', fontSize: '0.875rem', marginTop: '0.5rem' }}>
                      Menores de idade só podem criar despesas
                    </p>
                  )}
                </div>
              </div>

              <div className="modal-footer">
                <button
                  type="button"
                  className="btn-secondary"
                  onClick={handleCloseForm}
                >
                  Cancelar
                </button>
                <button type="submit" className="btn-primary">
                  {editingPerson ? 'Atualizar' : 'Criar'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Listagem de Pessoas */}
      {people && people.length > 0 ? (
        <>
          <div style={{ overflowX: 'auto' }}>
            <table>
              <thead>
                <tr>
                  <th>Nome</th>
                  <th>Idade</th>
                  <th className="text-right">Receitas</th>
                  <th className="text-right">Despesas</th>
                  <th className="text-right">Saldo</th>
                  <th className="text-center">Ações</th>
                </tr>
              </thead>
              <tbody>
                {people.map(person => (
                  <tr key={person.id}>
                    <td><strong>{person.name}</strong></td>
                    <td>{person.age} anos</td>
                    <td className="text-right text-success">R$ {(person.totalIncome ?? 0).toFixed(2)}</td>
                    <td className="text-right text-danger">R$ {(person.totalExpense ?? 0).toFixed(2)}</td>
                    <td className="text-right">
                      <span className={(person.balance ?? 0) >= 0 ? 'text-success' : 'text-danger'}>
                        R$ {(person.balance ?? 0).toFixed(2)}
                      </span>
                    </td>
                    <td className="text-center">
                      <button
                        className="btn-secondary btn-small"
                        onClick={() => handleEdit(person)}
                        style={{ marginRight: '0.5rem' }}
                      >
                        ✏️ Editar
                      </button>
                      <button
                        className="btn-danger btn-small"
                        onClick={() => handleDelete(person.id, person.name)}
                      >
                        🗑️ Deletar
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          {/* Resumo Total */}
          {summaryLoading ? (
            <div className="loading"><div className="spinner"></div></div>
          ) : summary && (
            <div className="card mt-4" style={{ backgroundColor: '#f0f9ff' }}>
              <h3>Resumo Total Geral</h3>
              <div className="grid grid-3 mt-3">
                <div style={{ textAlign: 'center', padding: '1rem' }}>
                  <div style={{ fontSize: '0.875rem', color: '#666', marginBottom: '0.5rem' }}>
                    Total Receitas
                  </div>
                  <div style={{ fontSize: '1.5rem', fontWeight: '600', color: '#10b981' }}>
                    R$ {(summary.totalIncome ?? 0).toFixed(2)}
                  </div>
                </div>
                <div style={{ textAlign: 'center', padding: '1rem' }}>
                  <div style={{ fontSize: '0.875rem', color: '#666', marginBottom: '0.5rem' }}>
                    Total Despesas
                  </div>
                  <div style={{ fontSize: '1.5rem', fontWeight: '600', color: '#ef4444' }}>
                    R$ {(summary.totalExpense ?? 0).toFixed(2)}
                  </div>
                </div>
                <div style={{ textAlign: 'center', padding: '1rem' }}>
                  <div style={{ fontSize: '0.875rem', color: '#666', marginBottom: '0.5rem' }}>
                    Saldo Líquido
                  </div>
                  <div style={{
                    fontSize: '1.5rem',
                    fontWeight: '600',
                    color: (summary.netBalance ?? 0) >= 0 ? '#10b981' : '#ef4444'
                  }}>
                    R$ {(summary.netBalance ?? 0).toFixed(2)}
                  </div>
                </div>
              </div>
            </div>
          )}
        </>
      ) : (
        <div className="card">
          <div className="empty-state">
            <h3>Nenhuma pessoa cadastrada</h3>
            <p>Clique em "Adicionar Pessoa" para começar</p>
          </div>
        </div>
      )}
    </div>
  );
};

export default PeopleSection;
