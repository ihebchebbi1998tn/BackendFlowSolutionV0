const Index = () => {
  return (
    <main className="flex min-h-screen items-center justify-center bg-gradient-to-br from-primary/10 via-background to-secondary/10">
      <div className="animate-in fade-in slide-in-from-bottom-4 duration-1000">
        <h1 className="text-7xl md:text-9xl font-bold bg-gradient-to-r from-primary via-secondary to-accent bg-clip-text text-transparent animate-in fade-in duration-1500">
          Hello World
        </h1>
        <p className="text-center text-muted-foreground mt-6 text-lg animate-in fade-in duration-2000">
          Welcome to your beautiful app
        </p>
      </div>
    </main>
  );
};

export default Index;
