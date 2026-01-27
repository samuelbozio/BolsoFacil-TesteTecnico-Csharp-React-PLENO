import axios, { AxiosInstance, AxiosError, AxiosResponse } from 'axios';
import LoggerService from './logger';
import {
  PersonDTO,
  PersonWithTotalsDTO,
  CategoryDTO,
  Category,
  CategoryWithTotalsDTO,
  TransactionDTO,
  TransactionResponseDTO,
  TotalSummaryDTO,
  ApiError,
  ApiResponse,
  TransactionType
} from '../types';

class ApiService {
  private api: AxiosInstance;

  constructor() {
    this.api = axios.create({
      baseURL: 'http://localhost:5000/api',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    LoggerService.info('ApiService inicializado', 'API_SERVICE');

    this.api.interceptors.request.use(
      (config) => {
        LoggerService.debug(
          `${config.method?.toUpperCase()} ${config.url}`,
          'API_REQUEST',
          { data: config.data }
        );
        return config;
      },
      (error) => {
        LoggerService.error('Erro ao fazer requisição', 'API_REQUEST', error);
        return Promise.reject(error);
      }
    );

    this.api.interceptors.response.use(
      (response: AxiosResponse) => {
        LoggerService.debug(
          `Resposta ${response.status}`,
          'API_RESPONSE',
          { data: response.data }
        );
        return response;
      },
      (error: AxiosError) => {
        const apiError: ApiError = {
          message: (error.response?.data as any)?.message || error.message || 'Erro ao conectar com o servidor',
          status: error.response?.status
        };
        LoggerService.error(
          `Erro ${error.response?.status}: ${apiError.message}`,
          'API_ERROR',
          error,
          { response: error.response?.data }
        );
        return Promise.reject(apiError);
      }
    );
  }

  getPeople = async (): Promise<PersonWithTotalsDTO[]> => {
    try {
      LoggerService.info('Obtendo todas as pessoas', 'API_PEOPLE');
      const response = await this.api.get<ApiResponse<PersonWithTotalsDTO[]>>('/people');
      const people = response.data.data;
      LoggerService.info(`${people.length} pessoas obtidas`, 'API_PEOPLE');
      return people;
    } catch (error) {
      LoggerService.error('Erro ao obter pessoas', 'API_PEOPLE', error);
      throw error;
    }
  }

  getPersonById = async (id: number): Promise<PersonWithTotalsDTO> => {
    try {
      LoggerService.info(`Obtendo pessoa com ID ${id}`, 'API_PEOPLE');
      const response = await this.api.get<ApiResponse<PersonWithTotalsDTO>>(`/people/${id}`);
      const person = response.data.data;
      LoggerService.info(`Pessoa ${person.name} obtida`, 'API_PEOPLE');
      return person;
    } catch (error) {
      LoggerService.error(`Erro ao obter pessoa ${id}`, 'API_PEOPLE', error);
      throw error;
    }
  }

  createPerson = async (personDto: PersonDTO): Promise<PersonWithTotalsDTO> => {
    try {
      LoggerService.info(`Criando pessoa: ${personDto.name}`, 'API_PEOPLE', personDto);
      const response = await this.api.post<ApiResponse<PersonWithTotalsDTO>>('/people', personDto);
      const person = response.data.data;
      LoggerService.info(`Pessoa ${person.name} criada com sucesso`, 'API_PEOPLE');
      return person;
    } catch (error) {
      LoggerService.error('Erro ao criar pessoa', 'API_PEOPLE', error, personDto);
      throw error;
    }
  }

  updatePerson = async (id: number, personDto: PersonDTO): Promise<PersonWithTotalsDTO> => {
    try {
      LoggerService.info(`Atualizando pessoa ${id}: ${personDto.name}`, 'API_PEOPLE', personDto);
      const response = await this.api.put<ApiResponse<PersonWithTotalsDTO>>(`/people/${id}`, personDto);
      const person = response.data.data;
      LoggerService.info(`Pessoa ${person.name} atualizada com sucesso`, 'API_PEOPLE');
      return person;
    } catch (error) {
      LoggerService.error(`Erro ao atualizar pessoa ${id}`, 'API_PEOPLE', error, personDto);
      throw error;
    }
  }

  deletePerson = async (id: number): Promise<void> => {
    try {
      LoggerService.info(`Deletando pessoa com ID ${id}`, 'API_PEOPLE');
      await this.api.delete(`/people/${id}`);
      LoggerService.info(`Pessoa ${id} deletada com sucesso`, 'API_PEOPLE');
    } catch (error) {
      LoggerService.error(`Erro ao deletar pessoa ${id}`, 'API_PEOPLE', error);
      throw error;
    }
  }

  getPeopleTotalSummary = async (): Promise<TotalSummaryDTO> => {
    try {
      LoggerService.info('Obtendo resumo total de pessoas', 'API_PEOPLE');
      const response = await this.api.get<ApiResponse<TotalSummaryDTO>>('/people/summary/totals');
      const summary = response.data.data;
      LoggerService.info('Resumo total obtido', 'API_PEOPLE', summary);
      return summary;
    } catch (error) {
      LoggerService.error('Erro ao obter resumo total', 'API_PEOPLE', error);
      throw error;
    }
  }

  getCategories = async (): Promise<Category[]> => {
    try {
      LoggerService.info('Obtendo todas as categorias', 'API_CATEGORIES');
      const response = await this.api.get<ApiResponse<Category[]>>('/categories');
      const categories = response.data.data;
      LoggerService.info(`${categories.length} categorias obtidas`, 'API_CATEGORIES');
      return categories;
    } catch (error) {
      LoggerService.error('Erro ao obter categorias', 'API_CATEGORIES', error);
      throw error;
    }
  }

  getCategoriesWithTotals = async (): Promise<CategoryWithTotalsDTO[]> => {
    try {
      LoggerService.info('Obtendo categorias com totais', 'API_CATEGORIES');
      const response = await this.api.get<ApiResponse<CategoryWithTotalsDTO[]>>('/categories/with-totals');
      const categories = response.data.data;
      LoggerService.info(`${categories.length} categorias com totais obtidas`, 'API_CATEGORIES');
      return categories;
    } catch (error) {
      LoggerService.error('Erro ao obter categorias com totais', 'API_CATEGORIES', error);
      throw error;
    }
  }

  createCategory = async (categoryDto: { description: string }): Promise<Category> => {
    try {
      LoggerService.info(`Criando categoria: ${categoryDto.description}`, 'API_CATEGORIES', categoryDto);
      const response = await this.api.post<ApiResponse<Category>>('/categories', categoryDto);
      const category = response.data.data;
      LoggerService.info(`Categoria criada com sucesso`, 'API_CATEGORIES');
      return category;
    } catch (error) { 
      LoggerService.error('Erro ao criar categoria', 'API_CATEGORIES', error, categoryDto);
      throw error;
    }
  }

  getTransactions = async (): Promise<TransactionResponseDTO[]> => {
    try {
      LoggerService.info('Obtendo todas as transações', 'API_TRANSACTIONS');
      // Server returns Application.DTOs.TransactionResponseDTO with fields: Amount, Type (string), CategoryName, PersonName
      const response = await this.api.get<ApiResponse<any[]>>('/transactions');
      const serverTxs = response.data.data || [];
      const transactions: TransactionResponseDTO[] = serverTxs.map((t: any) => ({
        id: t.id,
        description: t.description,
        value: t.amount ?? t.value,
        type: typeof t.type === 'string'
          ? (t.type === 'Income' ? TransactionType.Income : TransactionType.Expense)
          : t.type,
        categoryId: t.categoryId,
        categoryDescription: t.categoryName ?? t.categoryDescription ?? '',
        personId: t.personId,
        personName: t.personName ?? '',
        createdAt: t.createdAt
      }));
      LoggerService.info(`${transactions.length} transações obtidas`, 'API_TRANSACTIONS');
      return transactions;
    } catch (error) {
      LoggerService.error('Erro ao obter transações', 'API_TRANSACTIONS', error);
      throw error;
    }
  }

  createTransaction = async (transactionDto: TransactionDTO): Promise<TransactionResponseDTO> => {
    try {
      LoggerService.info(
        `Criando transação: ${transactionDto.type} de ${transactionDto.value}`,
        'API_TRANSACTIONS',
        transactionDto
      );
      // Server expects { value, type (number), ... } and returns Application.DTOs.TransactionResponseDTO
      const response = await this.api.post<ApiResponse<any>>('/transactions', transactionDto);
      const t = response.data.data;
      const transaction: TransactionResponseDTO = {
        id: t.id,
        description: t.description,
        value: t.amount ?? t.value,
        type: typeof t.type === 'string'
          ? (t.type === 'Income' ? TransactionType.Income : TransactionType.Expense)
          : t.type,
        categoryId: t.categoryId,
        categoryDescription: t.categoryName ?? t.categoryDescription ?? '',
        personId: t.personId,
        personName: t.personName ?? '',
        createdAt: t.createdAt
      };
      LoggerService.info(`Transação ${transactionDto.type} criada com sucesso`, 'API_TRANSACTIONS');
      return transaction;
    } catch (error) {
      LoggerService.error('Erro ao criar transação', 'API_TRANSACTIONS', error, transactionDto);
      throw error;
    }
  }
}

export const apiService = new ApiService();
export default apiService;
