/**
 * Tipos e Interfaces do Sistema
 * 
 * Este arquivo centraliza todas as definições de tipos utilizadas na aplicação,
 * garantindo type safety em toda a aplicação React.
 */

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}

// ========================================
// Enums
// ========================================

/**
 * Enum que define os tipos de transação
 * Expense: Despesa
 * Income: Receita
 */
export enum TransactionType {
  Expense = 0,
  Income = 1
}

/**
 * Enum que define as finalidades de uma categoria
 * Expense: Apenas despesas
 * Income: Apenas receitas
 * Both: Ambos os tipos
 */
export enum PurposeType {
  Expense = 0,
  Income = 1,
  Both = 2
}

// ========================================
// DTOs (Data Transfer Objects)
// ========================================

/**
 * Interface para criação/atualização de pessoa
 */
export interface PersonDTO {
  name: string;
  age: number;
}

/**
 * Interface para resposta de pessoa com totais
 * Inclui o cálculo de receitas, despesas e saldo
 */
export interface PersonWithTotalsDTO extends PersonDTO {
  id: number;
  totalIncome: number;
  totalExpense: number;
  balance: number;
}

/**
 * Interface para criação de categoria
 */
export interface CategoryDTO {
  description: string;
  purpose: PurposeType;
}

/**
 * Interface para resposta de categoria
 */
export interface Category extends CategoryDTO {
  id: number;
}

/**
 * Interface para resposta de categoria com totais
 */
export interface CategoryWithTotalsDTO extends Category {
  totalIncome: number;
  totalExpense: number;
  balance: number;
}

/**
 * Interface para criação de transação
 */
export interface TransactionDTO {
  description: string;
  value: number;
  type: TransactionType;
  categoryId: number;
  personId: number;
}

/**
 * Interface para resposta de transação
 */
export interface TransactionResponseDTO extends TransactionDTO {
  id: number;
  categoryDescription: string;
  personName: string;
  createdAt: string;
}

/**
 * Interface para resumo total geral
 * Contém totais globais de receitas, despesas e saldo
 */
export interface TotalSummaryDTO {
  totalIncome: number;
  totalExpense: number;
  netBalance: number;
}

// ========================================
// Estados de Erro
// ========================================

/**
 * Interface para tratamento de erros da API
 */
export interface ApiError {
  message: string;
  status?: number;
}
