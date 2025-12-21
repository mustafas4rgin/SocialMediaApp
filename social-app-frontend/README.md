# SocialApp Frontend

A modern, professional social media platform built with Next.js 15, React 19, and Tailwind CSS v4.

## 🚀 Features

- **Modern UI/UX**: Clean, professional design with glassmorphism effects
- **Authentication**: Login and registration pages (ready for backend integration)
- **Social Feed**: Post creation, likes, comments, and sharing
- **User Profiles**: Avatar, bio, and user information
- **Recommended Users**: Discover and follow new people
- **Trending Topics**: Explore popular hashtags
- **Responsive Design**: Works seamlessly on desktop and mobile devices
- **Dark Mode Ready**: Theme system prepared for dark mode implementation

## 🛠️ Tech Stack

- **Framework**: Next.js 15 (App Router)
- **UI Library**: React 19
- **Styling**: Tailwind CSS v4
- **Component Library**: shadcn/ui
- **State Management**: Zustand (configured)
- **API Client**: TanStack Query + Axios
- **Authentication**: NextAuth.js (ready for implementation)
- **Icons**: Lucide React
- **Date Formatting**: date-fns

## 📦 Installation

1. **Install dependencies**:
   ```bash
   npm install
   ```

2. **Set up environment variables**:
   ```bash
   cp .env.local.example .env.local
   ```
   
   Update the `.env.local` file with your backend API URL:
   ```
   NEXT_PUBLIC_API_URL=http://localhost:5000/api
   ```

3. **Run the development server**:
   ```bash
   npm run dev
   ```

4. **Open your browser**:
   Navigate to [http://localhost:3000](http://localhost:3000)

## 🎨 Design System

### Colors
- **Primary**: Twitter Blue (#0072BB) - Modern, trustworthy, engaging
- **Secondary**: Slate Grey (#708090) - Professional balance
- **Accent**: Light Blue (#E7F3FF) - Subtle highlights

### Typography
- **Headings**: Urbanist (Modern, bold, clean)
- **Body**: Lato (Friendly, professional, readable)

### Theme
The app uses CSS variables for theming, making it easy to customize colors and switch between light/dark modes.

## 📁 Project Structure

```
social-app-frontend/
├── src/
│   ├── app/
│   │   ├── (auth)/          # Authentication pages
│   │   │   ├── login/
│   │   │   └── register/
│   │   ├── (main)/          # Main app pages
│   │   │   └── feed/
│   │   ├── globals.css      # Global styles and theme
│   │   ├── layout.tsx       # Root layout
│   │   └── page.tsx         # Landing page
│   ├── components/
│   │   ├── feed/            # Feed-related components
│   │   ├── layout/          # Layout components
│   │   └── ui/              # shadcn/ui components
│   ├── lib/
│   │   ├── api.ts           # Axios configuration
│   │   ├── queries.ts       # API query functions
│   │   └── utils.ts         # Utility functions
│   └── types/
│       └── index.ts         # TypeScript types
├── public/                  # Static assets
└── package.json
```

## 🔌 Backend Integration

The frontend is ready to connect to your ASP.NET Core backend. Update the API endpoints in `src/lib/queries.ts` to match your backend routes.

### API Configuration

The API client is configured in `src/lib/api.ts` with:
- Automatic token refresh
- Request/response interceptors
- CORS support
- Error handling

### Available API Functions

Located in `src/lib/queries.ts`:
- **Auth**: login, register, logout, getCurrentUser
- **Users**: getUser, updateUser, followUser, unfollowUser
- **Posts**: getFeed, createPost, updatePost, deletePost, likePost
- **Comments**: getComments, createComment, deleteComment

## 🎯 Next Steps

1. **Connect to Backend**: Update `NEXT_PUBLIC_API_URL` in `.env.local`
2. **Implement Authentication**: Complete NextAuth.js setup
3. **Add Real Data**: Replace mock data with actual API calls
4. **User Profiles**: Create user profile pages
5. **Messaging**: Add direct messaging feature
6. **Notifications**: Implement real-time notifications
7. **Search**: Add search functionality
8. **Dark Mode**: Complete dark mode implementation

## 📱 Pages

- `/` - Landing page
- `/login` - Login page
- `/register` - Registration page
- `/feed` - Main social feed (requires auth)

## 🎨 Components

### Layout Components
- **Header**: Navigation bar with search, notifications, and user menu
- **Sidebar**: Recommended users and trending topics

### Feed Components
- **CreatePost**: Compose new posts
- **PostCard**: Display individual posts with likes, comments, and sharing
- **RecommendedUsers**: Suggested users to follow

## 🔧 Development

```bash
# Run development server
npm run dev

# Build for production
npm run build

# Start production server
npm start

# Run linter
npm run lint
```

## 📝 License

This project is part of SocialApp - a modern social media platform.

## 🤝 Contributing

This is a learning project. Feel free to explore and modify as needed!

---

Built with ❤️ using Next.js 15, React 19, and Tailwind CSS v4