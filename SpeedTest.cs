public class SpeedTest {

    public static final String FINISH_COMMAND = "finish";
    private static final ResourceBundle MESSAGES = ResourceBundle.getBundle("laborai.gui.messages");

    private static final String[] TYRIMU_VARDAI = {"contTree", "contHash", "remTree", "remHash", "avlCont", "bstCont"};
    private static final int[] TIRIAMI_KIEKIAI = {100000, 200000, 300000, 400000};

    private final BlockingQueue resultsLogger = new SynchronousQueue();
    private final Semaphore semaphore = new Semaphore(-1);
    private final Timekeeper tk;
    private final String[] errors;

    private final TreeSet<Integer> treeSet = new TreeSet<>();
    private final HashSet<Integer> hashSet = new HashSet<>();
    private final AvlSetKTU<Integer> avlset = new AvlSetKTU<>();
    private final BstSetKTU<Integer> bstset = new BstSetKTU<>();
    
    private final Random random = new Random();

    public SpeedTest() {
        semaphore.release();
        tk = new Timekeeper(TIRIAMI_KIEKIAI, resultsLogger, semaphore);
        errors = new String[]{
            MESSAGES.getString("error1"),
            MESSAGES.getString("error2"),
            MESSAGES.getString("error3"),
            MESSAGES.getString("error4")
        };
    }

    public void pradetiTyrima() {
        try {
            SisteminisTyrimas();
        } catch (InterruptedException e) {
            Thread.currentThread().interrupt();
        } catch (Exception ex) {
            ex.printStackTrace(System.out);
        }
    }

    public void SisteminisTyrimas() throws InterruptedException {
        try {
            for (int k : TIRIAMI_KIEKIAI) {
                treeSet.clear();
                hashSet.clear();
                avlset.clear();
                bstset.clear();
                ArrayList<Integer> array = new ArrayList<>(k);
                for(int i = 0; i<k; i++){
                    array.add(random.nextInt(k));
                }
                for (int i = 0; i < k; i++) {
                    treeSet.add(array.get(i));
                    hashSet.add(array.get(i));
                }
                tk.startAfterPause();
                tk.start();
                
                for(int i = 0; i < k; i++){
                    treeSet.contains(array.get(i));
                }
                tk.finish(TYRIMU_VARDAI[0]);
                
                for(int i = 0; i < k; i++){
                    hashSet.contains(array.get(i));
                }
                tk.finish(TYRIMU_VARDAI[1]);
                
                for(int i = 0; i < k; i++){
                    treeSet.remove(i);
                }
                tk.finish(TYRIMU_VARDAI[2]);
                
                for(int i = 0; i < k; i++){
                    hashSet.remove(i);
                }
                tk.finish(TYRIMU_VARDAI[3]);
                
                for(int i = 0; i < k; i++){
                    avlset.contains(i);
                }
                tk.finish(TYRIMU_VARDAI[4]);
                
                for(int i = 0; i < k; i++){
                    bstset.contains(i);
                }
                tk.finish(TYRIMU_VARDAI[5]);
                tk.seriesFinish();
            }
            tk.logResult(FINISH_COMMAND);
            
        } catch (MyException e) {
            if (e.getCode() >= 0 && e.getCode() <= 3) {
                tk.logResult(errors[e.getCode()] + ": " + e.getMessage());
            } else if (e.getCode() == 4) {
                tk.logResult(MESSAGES.getString("msg3"));
            } else {
                tk.logResult(e.getMessage());
            }
        }
    }

    public BlockingQueue<String> getResultsLogger() {
        return resultsLogger;
    }

    public Semaphore getSemaphore() {
        return semaphore;
    }
}
