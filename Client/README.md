# Frontend - Controle de Gastos Residenciais

Frontend React/TypeScript para o sistema de controle de gastos residenciais.

## 🎯 Objetivo

Fornecer uma interface completa e responsiva para gerenciar receitas e despesas domésticas, conectando-se a um backend .NET.

## 📦 Instalação

```bash
# Instalar dependências
npm install

# Iniciar servidor de desenvolvimento
npm run dev

# Build para produção
npm run build

# Visualizar build
npm run preview
```

## 🏗️ Estrutura do Projeto

### `/src/components`

Componentes React reutilizáveis:

- **`Header.tsx`** - Navegação principal e branding
  - Menu de navegação
  - Indicador de página ativa
  - Estilos responsivos

- **`Notification.tsx`** - Sistema de notificações
  - Toast com diferentes tipos (success, error, warning, info)
  - Container para múltiplas notificações
  - Dismissíveis automaticamente

- **`PeopleSection.tsx`** - Gerenciar pessoas
  - CRUD completo de pessoas
  - Listagem com totais de receitas e despesas
  - Modal para criação/edição
  - Validação de dados

- **`CategoriesSection.tsx`** - Gerenciar categorias
  - Criação de categorias
  - Filtro por finalidade
  - Listagem com totais por categoria
  - Resumo filtrado

- **`TransactionsSection.tsx`** - Gerenciar transações
  - Criação com validações
  - Filtro por tipo e pessoa
  - Validação de menores de idade
  - Validação de compatibilidade de categoria

- **`SummarySection.tsx`** - Relatórios
  - Resumo geral consolidado
  - Totais por pessoa
  - Totais por categoria
  - Estatísticas rápidas

### `/src/hooks`

Custom Hooks reutilizáveis:

```typescript
// useAsync - Gerencia requisições GET
const { data, loading, error, refetch } = useAsync(
  () => apiService.getPeople()
);

// useMutation - Gerencia POST/PUT/DELETE
const { mutate, loading, error } = useMutation(
  apiService.createPerson
);

// useForm - Gerencia estado de formulário
const form = useForm({ name: '', age: 0 });

// useNotification - Gerencia notificações
const { notify, success, error } = useNotification();

// useModal - Gerencia visibilidade de modal
const { isOpen, open, close } = useModal();
```

### `/src/services`

- **`apiService.ts`** - Cliente HTTP centralizado
  - Baseado em Axios
  - Interceptadores de erro
  - Métodos tipados para cada endpoint
  - Configuração de CORS

### `/src/types`

- **`index.ts`** - Definições de tipos TypeScript
  - Enums: `TransactionType`, `PurposeType`
  - DTOs: `PersonDTO`, `CategoryDTO`, etc.
  - Interfaces de erro

### `/src/styles`

- **`index.css`** - Estilos globais
  - Reset CSS
  - Componentes base (cards, botões, formulários)
  - Variáveis CSS
  - Responsividade

## 🎨 Componentes e Padrões

### Padrão de Componente

```typescript
/**
 * Descrição clara do componente
 * Explica funcionalidade, validações e dependências
 */

interface ComponentProps {
  prop1: string;
  prop2: number;
}

const Component: React.FC<ComponentProps> = ({ prop1, prop2 }) => {
  // Hooks primeiro
  const { data, loading } = useAsync(...);
  
  // Estado local
  const [localState, setLocalState] = useState(...);
  
  // Handlers
  const handleSubmit = async (e) => { ... };
  
  // Render condicional
  if (loading) return <LoadingComponent />;
  
  return (
    <div>
      {/* Conteúdo */}
    </div>
  );
};

export default Component;
```

### Tratamento de Erros

```typescript
try {
  await apiService.createPerson(data);
  notify('Sucesso!', 'success');
} catch (err: any) {
  notify(err.message || 'Erro genérico', 'error');
}
```

### Validações

```typescript
if (!form.values.name.trim()) {
  notify('Nome é obrigatório', 'warning');
  return;
}
```

## 🌐 Variáveis de Ambiente

Crie um arquivo `.env` na raiz do projeto (opcional):

```env
VITE_API_BASE_URL=https://localhost:7119/api
```

## 📱 Responsividade

Breakpoints utilizados:

- **Desktop**: 1200px+
- **Tablet**: 768px - 1199px
- **Mobile**: até 767px

Todos os componentes são 100% responsivos.

## 🔐 Segurança

- TypeScript para type safety
- Validação de entrada em formulários
- Validações de negócio do backend
- CORS configurado
- Tratamento seguro de erros

## ⚡ Performance

- Memoização de cálculos complexos
- Lazy loading apropriado
- Re-renders minimizados
- Código splitting automático com Vite

## 🧪 Testing

Para adicionar testes (recomendado):

```bash
npm install --save-dev vitest @testing-library/react
```

## 📚 Recursos Úteis

- [React Documentation](https://react.dev)
- [TypeScript Documentation](https://www.typescriptlang.org/docs/)
- [Axios Documentation](https://axios-http.com/docs)
- [Vite Documentation](https://vitejs.dev)

## 🐛 Problemas Comuns

### "Cannot GET /"
- Certifique-se de que o servidor backend está em execução
- Verifique a URL de conexão em `apiService.ts`

### Estilos não carregam
- Limpe o cache do navegador (Ctrl+Shift+Delete)
- Verifique se o CSS está sendo importado corretamente em `main.tsx`

### Transações da API ficam penduradas
- Verifique se o .NET server está em execução
- Verifique os logs do servidor
- Tente fazer rebuild: `npm run build`

## 📝 Convenções de Código

1. **Nomes de Arquivos**: PascalCase para componentes, camelCase para utilitários
2. **Nomes de Variáveis**: camelCase
3. **Nomes de Componentes**: PascalCase
4. **Comentários**: Use comentários descritivos em seções importantes
5. **Tipos**: Sempre declare tipos explicitamente

## 🚀 Build para Produção

```bash
# Build otimizado
npm run build

# Visualizar resultado
npm run preview

# Fazer deploy no servidor
# O arquivo dist/ contém tudo pronto para produção
```

---

**Parte do Sistema de Controle de Gastos | v1.0.0**
