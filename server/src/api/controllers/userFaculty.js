import userFacultyModel from "../models/userFacultyModel.js"
import bcrypt from 'bcrypt'
import generateToken from "../middlewares/generateToken.js"

/**
 * Route: /userFaculty
 * Desc: to show or access user Faculty
 */
export const userFaculty = async (req, res) => {
    res.status(200).json({message:"Show user Faculty signin/signup page"})

}



/**
 * Route: /userFaculty/signup
 * Desc: Faculty user sign up
 */
export const signup = async (req, res) => {
       const { 
        name, 
        email, 
        password, 
        confirmPassword, 
        branch,
        subjects,
        designation,
        education,
        bio,           //optional
        intrest,      //optional
        mobile       //optional
      } = req.body  

              
       //check if any field is not empty
       if (!name || !email || !password || !confirmPassword) {
        return res.status(404).json({
          success: false,
          message: "Please Fill all the Details.",
        });
      }
      
      //password and email constrains
      const passwordRegex =
      /^(?=.*[a-z])(?=.*[A-Z])(?=.*[@$%#^&*])(?=.*[0-9]).{8,}$/;

      const emailDomains = [
       "@gmail.com",
       "@yahoo.com",
       "@hotmail.com",
       "@aol.com",
       "@outlook.com",
       ];



       //check name length
       if (name.length < 2) {
         return res
           .status(404)
           .json({ message: "Name must be atleast 2 characters long." });
       }



       // check email format
       if (!emailDomains.some((v) => email.indexOf(v) >= 0)) {
         return res.status(404).json({
        message: "Please enter a valid email address",
      })};


      // check password format
       if (!passwordRegex.test(password)) {
         return res.status(404).json({
        message: "Password must be at least 8 characters long and include at least 1 uppercase letter, 1 lowercase letter, 1 symbol (@$%#^&*), and 1 number (0-9)",
        });
      }            
 
        
      // check password match
       if(password != confirmPassword){
         res.json({msg:"Password does not match"})
         }    
      
         
          // check password match
          if(password != confirmPassword){
            res.json({msg:"Password does not match"})
           } 
  
  
  
         /**
          * checking field types
          * to avoid sql attacks
          */
         if (typeof name !== "string" && name !== undefined) {
          res.status(400).json({ status: "error" });
          return;
        }
  
        if (typeof email !== "string" && email !== undefined) {
          res.status(400).json({ status: "error" });
          return;
        }
         
        if (typeof branch !== "string" && branch !== undefined) {
          res.status(400).json({ status: "error" });
          return;
        }
  
        if (typeof intrest !== "string" && intrest !== undefined) {
          res.status(400).json({ status: "error" });
          return;
        }
  
        if (typeof subjects !== "string" && subjects !== undefined) {
          res.status(400).json({ status: "error" });
          return;
        }
  
        if (typeof password !== "string" || typeof confirmPassword !== "string") {
          res.status(400).json({ status: "error" });
          return;
        }
  
        if (typeof designation !== "string" && designation !== undefined) {
          res.status(400).json({ status: "error" });
          return;
        }
  
        if (typeof mobile !== "number" && mobile !== undefined) {
          res.status(400).json({ status: "error" });
          return;
        }

        if (typeof bio !== "string" && bio !== undefined) {
          res.status(400).json({ status: "error" });
          return;
        }

        if (typeof education !== "number" && education !== undefined) {
          res.status(400).json({ status: "error" });
          return;
        }

        
        

       const oldUser = await userFacultyModel.findOne({ email });
       try{
        if(!oldUser){
 

          // hash password with bcrypt
           const hashedPassword = await bcrypt.hash(password, 12)

           // create userFaculty in database 
            const result = userFacultyModel.create({
                name,
                email,
                password: hashedPassword,
                branch,
                subjects,
                designation,
                education,
                bio,
                intrest,
                mobile
             });
    
             if(result){
                res.json({msg: "user Faculty added successfully"})
             }
           }
           else{
            res.json({msg: "user already exist"})
           }
      }
      catch(err){
        console.log(err)
      }

}




/**
 * Route: /userFaculty/signin
 * Desc: user Faculty sign in
 */
export const signin = async (req, res) => {
      const {email, password} = req.body  
      
      const oldUser = await userFacultyModel.findOne({email})

      const SECRET = process.env.FACULTY_SECRET
      
      if(oldUser){
        
        const isPasswordCorrect = bcrypt.compare(oldUser.password, password)

        if(isPasswordCorrect){
          
          const token = generateToken(oldUser, SECRET);

          req.session.user = {
            token: token,
            user: oldUser
          }

          res.status(200).json({
            success: true,
            result: oldUser,
            token,
            csrfToken: req.csrfToken,
            msg: "Faculty is logged in successfully"
          });

        }
        else{
            res.json({ msg: "Incorrect password" })
        }
      }
      else{
        res.json({ msg:"User Faculty does not exist" })
      }
}






