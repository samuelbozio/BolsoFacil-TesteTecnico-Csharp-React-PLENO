/**
 * Componente de Seção de Transações
 * 
 * Gerencia transações financeiras com:
 * - Listagem de todas as transações
 * - Criação de novas transações
 * - Validação de menores de idade (só despesas)
 * - Validação de compatibilidade entre categoria e tipo de transação
 * - Filtros por tipo (receita/despesa) e pessoa
 * - Exibição de informações completas de cada transação
 */

import React, { useState, useMemo } from 'react';
import { TransactionDTO, TransactionType } from '../types';
import { useAsync, useMutation, useForm, useNotification, useModal } from '../hooks/useCustomHooks';
import apiService from '../services/apiService';
import { NotificationContainer } from './Notification';

const TransactionsSection: React.FC = () => {
  const { data: transactions, refetch: refetchTransactions } = useAsync(() => apiService.getTransactions());
  const { data: people } = useAsync(() => apiService.getPeople());
  const { data: categories } = useAsync(() => apiService.getCategories());
  const { mutate: createTransaction } = useMutation(apiService.createTransaction);

  const { notifications, notify, removeNotification } = useNotification();
  const { isOpen: isFormOpen, open: openForm, close: closeForm } = useModal();
  const [typeFilter, setTypeFilter] = useState<'all' | TransactionType>('all');
  const [personFilter, setPersonFilter] = useState<'all' | number>('all');

  const form = useForm<TransactionDTO>({
    description: '',
    value: 0,
    type: TransactionType.Expense,
    categoryId: 0,
    personId: 0
  });

  const formatter = new Intl.NumberFormat("pt-BR", {
    style: "currency",
    currency: "BRL",
  });

  const compatibleCategories = useMemo(() => {
    if (!categories) return [];
    return categories.filter(cat => {
      if (form.values.type === TransactionType.Expense) {
        return cat.purpose === 0 || cat.purpose === 2; // Expense or Both
      } else {
        return cat.purpose === 1 || cat.purpose === 2; // Income or Both
      }
    });
  }, [categories, form.values.type]);

  const canCreateIncome = useMemo(() => {
    if (form.values.personId === 0 || !people) return true;
    const person = people.find(p => p.id === form.values.personId);
    if (!person) return true;
    return person.age >= 18;
  }, [form.values.personId, people]);

  const filteredTransactions = useMemo(() => {
    if (!transactions) return [];
    return transactions.filter(t => {
      const typeMatch = typeFilter === 'all' || t.type === typeFilter;
      const personMatch = personFilter === 'all' || t.personId === personFilter;
      return typeMatch && personMatch;
    });
  }, [transactions, typeFilter, personFilter]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!form.values.description.trim()) {
      notify('Por favor, preencha a descrição', 'warning');
      return;
    }

    if (form.values.value <= 0) {
      notify('O valor deve ser maior que zero', 'warning');
      return;
    }

    if (form.values.categoryId === 0) {
      notify('Por favor, selecione uma categoria', 'warning');
      return;
    }

    if (form.values.personId === 0) {
      notify('Por favor, selecione uma pessoa', 'warning');
      return;
    }

    if (form.values.type === TransactionType.Income && !canCreateIncome) {
      notify('Menores de idade (< 18 anos) só podem criar despesas', 'error');
      return;
    }

    try {
      await createTransaction(form.values);
      notify('Transação criada com sucesso!', 'success');
      form.reset();
      closeForm();
      refetchTransactions();
    } catch (err: any) {
      notify(err.message || 'Erro ao criar transação', 'error');
    }
  };

  const handleTypeChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    form.setValue('type', Number(e.target.value) as TransactionType);
    form.setValue('categoryId', 0); // Reset categoria ao mudar tipo
  };

  const typeLabels = {
    [TransactionType.Expense]: '💸 Despesa',
    [TransactionType.Income]: '📈 Receita'
  };

  return (
    <div>
      <NotificationContainer notifications={notifications} onRemove={removeNotification} />

      <div className="card">
        <div className="flex flex-between">
          <h2>Gerenciamento de Transações</h2>
          <button className="btn-primary" onClick={openForm}>+ Adicionar Transação</button>
        </div>
      </div>

      {/* Modal de Formulário */}
      {isFormOpen && (
        <div className="modal-overlay">
          <div className="modal">
            <div className="modal-header">
              <h3>Adicionar Transação</h3>
              <button className="close-btn" onClick={closeForm}>×</button>
            </div>

            <form onSubmit={handleSubmit}>
              <div className="modal-body">
                <div className="form-group">
                  <label htmlFor="person">Pessoa *</label>
                  <select
                    id="person"
                    name="personId"
                    value={form.values.personId}
                    onChange={(e) => form.setValue('personId', Number(e.target.value))}
                    required
                  >
                    <option value="0">Selecionar pessoa...</option>
                    {people?.map(p => (
                      <option key={p.id} value={p.id}>
                        {p.name} ({p.age} anos)
                      </option>
                    ))}
                  </select>
                </div>

                <div className="form-group">
                  <label htmlFor="type">Tipo de Transação *</label>
                  <select
                    id="type"
                    name="type"
                    value={form.values.type}
                    onChange={handleTypeChange}
                    required
                  >
                    <option value={TransactionType.Expense}>{typeLabels[TransactionType.Expense]}</option>
                    <option value={TransactionType.Income}>{typeLabels[TransactionType.Income]}</option>
                  </select>
                </div>

                {form.values.type === TransactionType.Income && !canCreateIncome && (
                  <div className="alert alert-warning" style={{ margin: '1rem 0' }}>
                    Menores de idade não podem criar receitas
                  </div>
                )}

                <div className="form-group">
                  <label htmlFor="category">Categoria *</label>
                  <select
                    id="category"
                    name="categoryId"
                    value={form.values.categoryId}
                    onChange={(e) => form.setValue('categoryId', Number(e.target.value))}
                    required
                  >
                    <option value="0">Selecionar categoria...</option>
                    {compatibleCategories?.map(c => (
                      <option key={c.id} value={c.id}>
                        {c.description}
                      </option>
                    ))}
                  </select>
                  {compatibleCategories?.length === 0 && form.values.categoryId === 0 && (
                    <p style={{ color: '#ef4444', fontSize: '0.875rem', marginTop: '0.5rem' }}>
                      Nenhuma categoria compatível com este tipo de transação
                    </p>
                  )}
                </div>

                <div className="form-group">
                  <label htmlFor="description">Descrição *</label>
                  <input
                    id="description"
                    name="description"
                    type="text"
                    value={form.values.description}
                    onChange={form.handleChange}
                    placeholder="Ex: Compra no supermercado"
                    maxLength={400}
                    required
                  />
                </div>

                <div className="form-group">
                  <label htmlFor="value">Valor (R$) *</label>
                  <input
                    id="value"
                    name="value"
                    type="text"
                    inputMode="decimal"
                    placeholder="R$ 0,00"
                    value={formatter.format(form.values.value || 0)}
                    onChange={(e) => {
                      const raw = e.target.value.replace(/\D/g, "");
                      const number = Number(raw) / 100;

                      form.setValue("value", number);
                    }}
                    required
                  />
                </div>
              </div>

              <div className="modal-footer">
                <button type="button" className="btn-secondary" onClick={closeForm}>
                  Cancelar
                </button>
                <button
                  type="submit"
                  className="btn-primary"
                  disabled={!canCreateIncome && form.values.type === TransactionType.Income}
                >
                  Criar
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Filtros */}
      {transactions && transactions.length > 0 && (
        <div className="card mb-3">
          <div className="grid grid-2">
            <div>
              <label style={{ marginBottom: '0.5rem', display: 'block', fontWeight: '500' }}>
                Tipo:
              </label>
              <div className="flex" style={{ gap: '0.5rem', flexWrap: 'wrap' }}>
                <button
                  className="btn-secondary btn-small"
                  onClick={() => setTypeFilter('all')}
                  style={{
                    backgroundColor: typeFilter === 'all' ? '#2563eb' : 'var(--light-bg)',
                    color: typeFilter === 'all' ? 'white' : 'var(--text-primary)'
                  }}
                >
                  Todas
                </button>
                <button
                  className="btn-secondary btn-small"
                  onClick={() => setTypeFilter(TransactionType.Expense)}
                  style={{
                    backgroundColor: typeFilter === TransactionType.Expense ? '#ef4444' : 'var(--light-bg)',
                    color: typeFilter === TransactionType.Expense ? 'white' : 'var(--text-primary)'
                  }}
                >
                  Despesas
                </button>
                <button
                  className="btn-secondary btn-small"
                  onClick={() => setTypeFilter(TransactionType.Income)}
                  style={{
                    backgroundColor: typeFilter === TransactionType.Income ? '#10b981' : 'var(--light-bg)',
                    color: typeFilter === TransactionType.Income ? 'white' : 'var(--text-primary)'
                  }}
                >
                  Receitas
                </button>
              </div>
            </div>

            <div>
              <label htmlFor="personFilterSelect" style={{ marginBottom: '0.5rem', display: 'block', fontWeight: '500' }}>
                Pessoa:
              </label>
              <select
                id="personFilterSelect"
                value={personFilter === 'all' ? 'all' : personFilter}
                onChange={(e) => setPersonFilter(e.target.value === 'all' ? 'all' : Number(e.target.value))}
                style={{ maxWidth: '100%' }}
              >
                <option value="all">Todas as pessoas</option>
                {people?.map(p => (
                  <option key={p.id} value={p.id}>
                    {p.name}
                  </option>
                ))}
              </select>
            </div>
          </div>
        </div>
      )}

      {/* Listagem */}
      {filteredTransactions && filteredTransactions.length > 0 ? (
        <div style={{ overflowX: 'auto' }}>
          <table>
            <thead>
              <tr>
                <th>Descrição</th>
                <th>Pessoa</th>
                <th>Categoria</th>
                <th className="text-right">Valor</th>
                <th>Tipo</th>
                <th>Data</th>
              </tr>
            </thead>
            <tbody>
              {filteredTransactions.map(transaction => (
                <tr key={transaction.id}>
                  <td>{transaction.description}</td>
                  <td>{transaction.personName}</td>
                  <td>{transaction.categoryDescription}</td>
                  <td className="text-right">
                    <strong>R$ {(transaction.value ?? 0).toFixed(2)}</strong>
                  </td>
                  <td>
                    <span style={{
                      display: 'inline-block',
                      padding: '0.25rem 0.75rem',
                      borderRadius: '0.25rem',
                      backgroundColor: transaction.type === TransactionType.Expense ? '#fee2e2' : '#dcfce7',
                      color: transaction.type === TransactionType.Expense ? '#991b1b' : '#166534',
                      fontSize: '0.875rem',
                      fontWeight: '500'
                    }}>
                      {typeLabels[transaction.type]}
                    </span>
                  </td>
                  <td>{new Date(transaction.createdAt).toLocaleDateString('pt-BR')}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      ) : (
        <div className="card">
          <div className="empty-state">
            <h3>Nenhuma transação encontrada</h3>
            <p>Clique em "Adicionar Transação" para começar</p>
          </div>
        </div>
      )}
    </div>
  );
};

export default TransactionsSection;
