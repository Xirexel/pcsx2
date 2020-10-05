package com.xirexel.omegared.Panels;

import android.app.Activity;
import android.content.Intent;
import android.content.pm.ActivityInfo;
import android.net.Uri;
import android.os.Bundle;
import android.view.View;
import android.widget.AdapterView;
import android.widget.Button;
import android.widget.ListView;

import com.xirexel.omegared.Adapters.*;
import com.xirexel.omegared.GameController;
import com.xirexel.omegared.Managers.*;
import com.xirexel.omegared.R;
import com.xirexel.omegared.Util.RealPathUtils;

import java.io.File;

import static android.view.View.VISIBLE;

public class ControlPanel extends Activity {

    private static final int READ_BIOS_REQUEST_CODE = 42;

    private static final int READ_GAME_REQUEST_CODE = 43;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE);

        overridePendingTransition(R.anim.fadein, R.anim.fadeout);

        setContentView(R.layout.activity_control_panel);

        Button l_controlBtn = findViewById(R.id.controlBtnHide);

        if(l_controlBtn != null)
        {
            l_controlBtn.setOnClickListener(
                    new View.OnClickListener(){
                        @Override
                        public void onClick(View v) {
                            finishAfterTransition();
                        }
                    }

            );
        }


        Button l_selectBIOSbtn = findViewById(R.id.selectBIOSBtn);

        if(l_selectBIOSbtn != null)
        {
            l_selectBIOSbtn.setOnClickListener(new View.OnClickListener(){
                                                   @Override
                                                   public void onClick(View v) {
                                                       switchToBIOS();
                                                   }
                                               }

            );
        }


        Button l_selectGameBtn = findViewById(R.id.selectGameBtn);

        if(l_selectGameBtn != null)
        {
            l_selectGameBtn.setOnClickListener(new View.OnClickListener(){
                                                   @Override
                                                   public void onClick(View v) {
                                                       switchToGame();
                                                   }
                                               }

            );
        }


        Button l_startPauseBtn = findViewById(R.id.startPauseBtn);

        if(l_startPauseBtn != null)
        {
            l_startPauseBtn.setOnClickListener(new View.OnClickListener(){
                                                   @Override
                                                   public void onClick(View v) {
                                                       GameController.getInstance().PlayPause();
                                                   }
                                               }

            );
        }




        Button l_stopBtn = findViewById(R.id.stopBtn);

        if(l_stopBtn != null)
        {
            l_stopBtn.setOnClickListener(new View.OnClickListener(){
                                                   @Override
                                                   public void onClick(View v) {
                                                       GameController.getInstance().Stop();
                                                   }
                                               }

            );
        }


    }

    private void switchToBIOS()
    {
        ListView l_listView = findViewById(R.id.controlListView);

        if(l_listView != null)
        {

            l_listView.setAdapter(BIOSAdapter.getInstance());

            l_listView.setItemChecked(BIOSAdapter.getInstance().selectedItem(), true);

            l_listView.setOnItemClickListener(
                    new AdapterView.OnItemClickListener() {
                        @Override
                        public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                            BIOSAdapter.getInstance().selectItem((int)id);
                        }
                    }
            );


            l_listView.setOnItemLongClickListener(
                    new AdapterView.OnItemLongClickListener() {
                        @Override
                        public boolean onItemLongClick(AdapterView<?> parent, View view, int position, long id) {
                            BIOSAdapter.getInstance().removeItem((int)id);
                            return false;
                        }
                    }

            );


            Button l_addItemBtn = findViewById(R.id.addItemBtn);

            if(l_addItemBtn != null)
            {
                l_addItemBtn.setVisibility(VISIBLE);

                l_addItemBtn.setOnClickListener(
                        new View.OnClickListener(){
                            @Override
                            public void onClick(View v) {
                                performBIOSFileSearch();
                            }
                        }

                );
            }
        }
    }

    private void switchToGame()
    {
        ListView l_listView = findViewById(R.id.controlListView);

        if(l_listView != null)
        {

            l_listView.setAdapter(ISOAdapter.getInstance());

            l_listView.setItemChecked(ISOAdapter.getInstance().selectedItem(), true);

            l_listView.setOnItemClickListener(
                    new AdapterView.OnItemClickListener() {
                        @Override
                        public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                            ISOAdapter.getInstance().selectItem((int)id);
                        }
                    }
            );


            l_listView.setOnItemLongClickListener(
                    new AdapterView.OnItemLongClickListener() {
                        @Override
                        public boolean onItemLongClick(AdapterView<?> parent, View view, int position, long id) {
                            ISOAdapter.getInstance().removeItem((int)id);
                            return false;
                        }
                    }

            );

            Button l_addItemBtn = findViewById(R.id.addItemBtn);

            if(l_addItemBtn != null)
            {
                l_addItemBtn.setVisibility(VISIBLE);

                l_addItemBtn.setOnClickListener(
                        new View.OnClickListener(){
                            @Override
                            public void onClick(View v) {
                                performGameFileSearch();
                            }
                        }

                );
            }
        }
    }


    public void performGameFileSearch() {

        Intent intent = new Intent(Intent.ACTION_GET_CONTENT);

        // Filter to only show results that can be "opened", such as a
        // file (as opposed to a list of contacts or timezones)
        intent.addCategory(Intent.CATEGORY_OPENABLE);

        // Filter to show only images, using the image MIME data type.
        // If one wanted to search for ogg vorbis files, the type would be "audio/ogg".
        // To search for all documents available via installed storage providers,
        // it would be "*/*".
        intent.setType("*/*");

        startActivityForResult(intent, READ_GAME_REQUEST_CODE);
    }


    public void performBIOSFileSearch() {

        Intent intent = new Intent(Intent.ACTION_GET_CONTENT);

        // Filter to only show results that can be "opened", such as a
        // file (as opposed to a list of contacts or timezones)
        intent.addCategory(Intent.CATEGORY_OPENABLE);

        // Filter to show only images, using the image MIME data type.
        // If one wanted to search for ogg vorbis files, the type would be "audio/ogg".
        // To search for all documents available via installed storage providers,
        // it would be "*/*".
        intent.setType("*/*");

        startActivityForResult(intent, READ_BIOS_REQUEST_CODE);
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode,
                                 Intent resultData) {


        if (requestCode == READ_BIOS_REQUEST_CODE && resultCode == Activity.RESULT_OK) {
            if (resultData != null) {
                Uri uri = resultData.getData();

                String l_path = null;
                try {
                    l_path = RealPathUtils.getRealPathFromURI_API19(getBaseContext(), uri);
                } catch (Exception e) {
                    e.printStackTrace();
                }

                File file = new File(l_path);

                if(file.exists())
                {
                    BiosManager.getInstance().addBIOS(l_path);
                }


//                Uri l_uri = getContentResolver().canonicalize(uri);

//                InputStream l_BIOSStream = null;
//
//                try {
//                    l_BIOSStream = getContentResolver().openInputStream(uri);
//
//                } catch (FileNotFoundException e) {
//                    e.printStackTrace();
//                }
//
//                BiosManager.getInstance().readBios(l_BIOSStream);


            }
        }
        else
            if (requestCode == READ_GAME_REQUEST_CODE && resultCode == Activity.RESULT_OK) {
            if (resultData != null) {
                Uri uri = resultData.getData();

                String l_path = null;
                try {
                    l_path = RealPathUtils.getRealPathFromURI_API19(getBaseContext(), uri);
                } catch (Exception e) {
                    e.printStackTrace();
                }

                File file = new File(l_path);

                if(file.exists())
                {
                    IsoManager.getInstance().addGame(l_path);
                }
            }
        }
    }
}
