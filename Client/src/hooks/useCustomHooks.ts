/**
 * Hooks Customizados da Aplicação
 * 
 * Este arquivo contém hooks reutilizáveis que encapsulam lógica comum
 * de estado e efeitos colaterais, promovendo code reuse e mantendo
 * componentes mais limpos.
 */

import { useState, useCallback, useEffect } from 'react';
import { ApiError } from '../types';

/**
 * Hook para gerenciar estado de requisições assíncronas
 * 
 * @template T Tipo de dado que será retornado
 * @param asyncFunction Função assíncrona a ser executada
 * @returns Objeto com dados, carregamento, erro e função de recarregar
 */
export function useAsync<T>(
  asyncFunction: () => Promise<T>
) {
  const [data, setData] = useState<T | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<ApiError | null>(null);

  // Memoiza a função assíncrona para evitar loops infinitos
  const memoizedAsyncFunction = useCallback(asyncFunction, []);

  // Carrega dados ao montar o componente (apenas uma vez)
  useEffect(() => {
    let isMounted = true;

    const load = async () => {
      setLoading(true);
      setError(null);
      try {
        const result = await memoizedAsyncFunction();
        if (isMounted) {
          setData(result);
        }
      } catch (err) {
        if (isMounted) {
          setError(err as ApiError);
        }
      } finally {
        if (isMounted) {
          setLoading(false);
        }
      }
    };

    load();

    return () => {
      isMounted = false;
    };
  }, [memoizedAsyncFunction]);

  const refetch = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const result = await memoizedAsyncFunction();
      setData(result);
    } catch (err) {
      setError(err as ApiError);
    } finally {
      setLoading(false);
    }
  }, [memoizedAsyncFunction]);

  return { data, loading, error, refetch };
}

/**
 * Hook para gerenciar operações que modificam dados (POST, PUT, DELETE)
 * 
 * @template T Tipo de dado retornado pela operação
 * @param mutationFn Função assíncrona a ser executada
 * @returns Objeto com função mutate, carregamento, erro e dados
 */
export function useMutation<T>(
  mutationFn: (...args: any[]) => Promise<T>
) {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<ApiError | null>(null);
  const [data, setData] = useState<T | null>(null);

  const mutate = useCallback(
    async (...args: any[]) => {
      setLoading(true);
      setError(null);
      try {
        const result = await mutationFn(...args);
        setData(result);
        return result;
      } catch (err) {
        const apiError = err as ApiError;
        setError(apiError);
        throw apiError;
      } finally {
        setLoading(false);
      }
    },
    [mutationFn]
  );

  return { mutate, loading, error, data };
}

/**
 * Hook para gerenciar estado de formulário simples
 * 
 * @template T Tipo do objeto de formulário
 * @param initialValues Valores iniciais do formulário
 * @returns Objeto com valores, setter, reset e handleChange
 */
export function useForm<T extends Record<string, any>>(initialValues: T) {
  const [values, setValues] = useState<T>(initialValues);

  const handleChange = useCallback(
    (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
      const { name, value, type } = e.target;

      setValues((prev: T) => ({
        ...prev,
        [name]: type === 'number' ? Number(value) : value
      }));
    },
    []
  );

  const reset = useCallback(() => {
    setValues(initialValues);
  }, [initialValues]);

  const setValue = useCallback((key: keyof T, value: any) => {
    setValues((prev: T) => ({
      ...prev,
      [key]: value
    }));
  }, []);

  return {
    values,
    handleChange,
    reset,
    setValue,
    setValues
  };
}

/**
 * Hook para gerenciar notificações/toasts
 * 
 * @returns Objeto com métodos para mostrar diferentes tipos de notificação
 */
export function useNotification() {
  const [notifications, setNotifications] = useState<
    Array<{ id: string; message: string; type: 'success' | 'error' | 'warning' | 'info'; duration?: number }>
  >([]);

  const notify = useCallback(
    (message: string, type: 'success' | 'error' | 'warning' | 'info' = 'info', duration = 5000) => {
      const id = `${Date.now()}-${Math.random()}`;

      setNotifications((prev) => [...prev, { id, message, type, duration }]);

      if (duration > 0) {
        setTimeout(() => {
          setNotifications((prev) => prev.filter((n) => n.id !== id));
        }, duration);
      }

      return id;
    },
    []
  );

  const removeNotification = useCallback((id: string) => {
    setNotifications((prev) => prev.filter((n) => n.id !== id));
  }, []);

  return {
    notifications,
    notify,
    removeNotification,
    success: (msg: string) => notify(msg, 'success'),
    error: (msg: string) => notify(msg, 'error'),
    warning: (msg: string) => notify(msg, 'warning'),
    info: (msg: string) => notify(msg, 'info')
  };
}

/**
 * Hook para gerenciar modal
 * 
 * @param initialOpen Estado inicial do modal
 * @returns Objeto com isOpen e funções open/close/toggle
 */
export function useModal(initialOpen = false) {
  const [isOpen, setIsOpen] = useState(initialOpen);

  const open = useCallback(() => setIsOpen(true), []);
  const close = useCallback(() => setIsOpen(false), []);
  const toggle = useCallback(() => setIsOpen((prev) => !prev), []);

  return { isOpen, open, close, toggle };
}
